using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;

namespace HotelReservasapp.BusinessLogic.Services;

public sealed class ReservationService : IReservationService
{
    private const decimal MaxDatabaseAmount = 9999999999.99m;

    private readonly IReservationRepository _reservationRepository;
    private readonly IBedroomRepository _bedroomRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IFloorRepository _floorRepository;
    private readonly INotificationService _notificationService;

    public ReservationService(IReservationRepository reservationRepository, IBedroomRepository bedroomRepository, IGuestRepository guestRepository, IHotelRepository hotelRepository, IFloorRepository floorRepository, INotificationService notificationService)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));

        _bedroomRepository = bedroomRepository ?? throw new ArgumentNullException(nameof(bedroomRepository));

        _guestRepository = guestRepository ?? throw new ArgumentNullException(nameof(guestRepository));

        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));

        _floorRepository = floorRepository ?? throw new ArgumentNullException(nameof(floorRepository));

        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _reservationRepository.GetAllAsync();
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        ValidateId(id);

        return await _reservationRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Bedroom>> GetAvailableAsync(int idHotel, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes)
    {
        ValidateId(idHotel);
        ValidateGuestCount(cantidadHuespedes);

        var (entrada, salida) = ValidateDates(fechaEntrada, fechaSalida);

        var hotel = await _hotelRepository.GetByIdAsync(idHotel) ?? throw new KeyNotFoundException("El hotel solicitado no existe.");

        if (hotel.Estado != GeneralState.Activo)
            throw new InvalidOperationException("No se pueden realizar reservas en un hotel inactivo.");

        return await _bedroomRepository.GetAvailableAsync(idHotel, entrada, salida, cantidadHuespedes);
    }

    public async Task<Reservation> CreateAsync(int idHabitacion, int idHuesped, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes)
    {
        ValidateId(idHabitacion);
        ValidateId(idHuesped);

        ValidateGuestCount(cantidadHuespedes);

        var (entrada, salida) = ValidateDates(fechaEntrada, fechaSalida);

        var guest = await _guestRepository.GetByIdAsync(idHuesped);

        if (guest is null)
            throw new KeyNotFoundException("El huésped solicitado no existe.");

        var bedroom = await EnsureBookableBedroomAsync(idHabitacion, entrada, salida, cantidadHuespedes);

        var total = CalculateTotal(bedroom.TarifaPorNoche, entrada, salida);

        var reservation = new Reservation
        {
            IdHabitacion = bedroom.Id,
            IdHuesped = guest.Id,
            FechaEntrada = entrada,
            FechaSalida = salida,
            CantidadHuespedes = cantidadHuespedes,
            TarifaPorNoche = bedroom.TarifaPorNoche,
            Total = total,
            Estado = ReservaState.Pendiente
        };

        await _reservationRepository.AddIfNoActiveOverlapAsync(reservation);

        return reservation;
    }

    public async Task<Reservation> ConfirmAsync(int id)
    {
        ValidateId(id);

        var reservation = await GetRequiredReservationAsync(id);

        if (reservation.Estado != ReservaState.Pendiente)
            throw new InvalidOperationException("Solamente se pueden confirmar reservas pendientes.");

        var guest = await _guestRepository.GetByIdAsync(reservation.IdHuesped);

        if (guest is null)
            throw new InvalidOperationException("El huésped de la reserva no existe.");

        var (entrada, salida) = ValidateDates(reservation.FechaEntrada, reservation.FechaSalida);

        await EnsureBookableBedroomAsync(reservation.IdHabitacion, entrada, salida, reservation.CantidadHuespedes, reservation.Id);

        reservation.Estado = ReservaState.Confirmada;

        await _reservationRepository.UpdateIfNoActiveOverlapAsync(reservation);

        await RegisterNotificationSafelyAsync(reservation.Id, NotificacionType.Confirmacion);

        return reservation;
    }

    public async Task<Reservation> ModifyAsync(int id, int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes)
    {
        ValidateId(id);
        ValidateId(idHabitacion);
        ValidateGuestCount(cantidadHuespedes);

        var (entrada, salida) = ValidateDates(fechaEntrada, fechaSalida);

        var reservation = await GetRequiredReservationAsync(id);

        if (reservation.Estado != ReservaState.Pendiente && reservation.Estado != ReservaState.Confirmada)
        {
            throw new InvalidOperationException("La reserva no se encuentra en un estado modificable.");
        }

        var hasSignificantChanges = reservation.IdHabitacion != idHabitacion || reservation.FechaEntrada.Date != entrada || reservation.FechaSalida.Date != salida || reservation.CantidadHuespedes != cantidadHuespedes;

        var bedroom = await EnsureBookableBedroomAsync(idHabitacion, entrada, salida, cantidadHuespedes, id);

        var total = CalculateTotal(bedroom.TarifaPorNoche, entrada, salida);

        reservation.IdHabitacion = bedroom.Id;
        reservation.FechaEntrada = entrada;
        reservation.FechaSalida = salida;
        reservation.CantidadHuespedes = cantidadHuespedes;
        reservation.TarifaPorNoche = bedroom.TarifaPorNoche;
        reservation.Total = total;

        await _reservationRepository.UpdateIfNoActiveOverlapAsync(reservation);

        if (hasSignificantChanges)
        {
            await RegisterNotificationSafelyAsync(reservation.Id, NotificacionType.Modificacion);
        }

        return reservation;
    }

    public async Task<Reservation> CancelAsync(int id)
    {
        ValidateId(id);

        var reservation = await GetRequiredReservationAsync(id);

        if (reservation.Estado != ReservaState.Pendiente && reservation.Estado != ReservaState.Confirmada)
        {
            throw new InvalidOperationException("La reserva no puede cancelarse en su estado actual.");
        }

        reservation.Estado = ReservaState.Cancelada;

        await _reservationRepository.UpdateIfNoActiveOverlapAsync(reservation);

        await RegisterNotificationSafelyAsync(reservation.Id, NotificacionType.Cancelacion);

        return reservation;
    }

    //Validaciones y métodos auxiliares
    private async Task<Bedroom> EnsureBookableBedroomAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes, int? idReservaExcluir = null)
    {
        var bedroom = await _bedroomRepository.GetByIdAsync(idHabitacion) ?? throw new KeyNotFoundException("La habitación solicitada no existe.");

        if (bedroom.Estado != BedroomState.Disponible)
            throw new InvalidOperationException("La habitación no está habilitada para reservas.");

        if (bedroom.Capacidad < cantidadHuespedes)
            throw new InvalidOperationException("La cantidad de huéspedes supera la capacidad de la habitación.");

        var hotel = await _hotelRepository.GetByIdAsync(bedroom.IdHotel);

        if (hotel is null || hotel.Estado != GeneralState.Activo)
            throw new InvalidOperationException("El hotel no existe o se encuentra inactivo.");

        var floor = await _floorRepository.GetByIdAsync(bedroom.IdPiso);

        if (floor is null || floor.IdHotel != bedroom.IdHotel || floor.Estado != GeneralState.Activo)
        {
            throw new InvalidOperationException("El piso no pertenece a un hotel activo o se encuentra inactivo.");
        }

        var hasOverlap = await _reservationRepository.HasActiveOverlapAsync(bedroom.Id, fechaEntrada, fechaSalida, idReservaExcluir);

        if (hasOverlap)
            throw new InvalidOperationException("La habitación ya tiene una reserva activa para ese período.");

        return bedroom;
    }

    private async Task<Reservation> GetRequiredReservationAsync(int id)
    {
        return await _reservationRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("La reserva solicitada no existe.");
    }

    private static decimal CalculateTotal(decimal tarifaPorNoche, DateTime fechaEntrada, DateTime fechaSalida)
    {
        var noches = (fechaSalida.Date - fechaEntrada.Date).Days;

        var total = decimal.Round(tarifaPorNoche * noches, 2, MidpointRounding.AwayFromZero);

        if (total > MaxDatabaseAmount)
            throw new InvalidOperationException("El total supera el valor máximo permitido.");

        return total;
    }

    private static (DateTime entrada, DateTime salida) ValidateDates(DateTime fechaEntrada, DateTime fechaSalida)
    {
        var entrada = fechaEntrada.Date;
        var salida = fechaSalida.Date;

        if (salida <= entrada)
            throw new ArgumentException("La fecha de salida debe ser posterior a la fecha de entrada.");

        if (entrada < DateTime.Today)
            throw new ArgumentException("No se pueden realizar reservas para fechas pasadas.");

        return (entrada, salida);
    }

    private static void ValidateGuestCount(short cantidadHuespedes)
    {
        if (cantidadHuespedes <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidadHuespedes),"La cantidad de huéspedes debe ser mayor que cero.");
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id),"El identificador debe ser mayor que cero.");
    }

    private async Task RegisterNotificationSafelyAsync(int idReserva, NotificacionType tipo)
    {
        try
        {
            await _notificationService.RegisterAsync(idReserva, tipo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"No se pudo registrar la notificación de la reserva " + $"{idReserva}. Tipo: {tipo}. Error: {ex}");
        }
    }
}