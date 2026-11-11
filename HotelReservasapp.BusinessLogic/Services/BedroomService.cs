using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;

namespace HotelReservasapp.BusinessLogic.Services;

public sealed class BedroomService : IBedroomService
{
    private readonly IBedroomRepository _bedroomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IFloorRepository _floorRepository;
    private readonly IBedroomTypeRepository _bedroomTypeRepository;

    public BedroomService(IBedroomRepository bedroomRepository, IHotelRepository hotelRepository, IFloorRepository floorRepository, IBedroomTypeRepository bedroomTypeRepository)
    {
        _bedroomRepository = bedroomRepository ?? throw new ArgumentNullException(nameof(bedroomRepository));

        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));

        _floorRepository = floorRepository ?? throw new ArgumentNullException(nameof(floorRepository));

        _bedroomTypeRepository = bedroomTypeRepository ?? throw new ArgumentNullException(nameof(bedroomTypeRepository));
    }

    public async Task<IEnumerable<Bedroom>> GetAllAsync()
    {
        return await _bedroomRepository.GetAllAsync();
    }

    public async Task<Bedroom?> GetByIdAsync(int id)
    {
        ValidateId(id);

        return await _bedroomRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Bedroom>> GetByHotelAsync(int idHotel)
    {
        ValidateId(idHotel);

        await EnsureHotelExistsAsync(idHotel);

        return await _bedroomRepository.GetByHotelAsync(idHotel);
    }

    public async Task<IEnumerable<Bedroom>> GetByFloorAsync(int idPiso)
    {
        ValidateId(idPiso);

        var floor = await _floorRepository.GetByIdAsync(idPiso) ?? throw new KeyNotFoundException("El piso solicitado no existe.");

        return await _bedroomRepository.GetByPisoAsync(idPiso);
    }

    public async Task<Bedroom> CreateAsync(int idHotel, int idPiso, int idTipoHabitacion, short numero, short capacidad, decimal tarifaPorNoche)
    {
        ValidateId(idHotel);
        ValidateId(idPiso);
        ValidateId(idTipoHabitacion);

        ValidateRoomNumber(numero);
        ValidateCapacity(capacidad);
        ValidateRate(tarifaPorNoche);

        await EnsureHotelExistsAsync(idHotel);

        await EnsureValidFloorAsync(idPiso, idHotel, requireActive: true);

        await EnsureBedroomTypeExistsAsync(idTipoHabitacion);

        await EnsureUniqueRoomNumberAsync(idHotel, numero);

        var bedroom = new Bedroom
        {
            IdHotel = idHotel,
            IdPiso = idPiso,
            IdTipoHabitacion = idTipoHabitacion,
            Numero = numero,
            Capacidad = capacidad,
            TarifaPorNoche = tarifaPorNoche,
            Estado = BedroomState.Disponible
        };

        await _bedroomRepository.AddAsync(bedroom);

        return bedroom;
    }

    public async Task<Bedroom> UpdateAsync(int id, int idPiso, int idTipoHabitacion, short numero, short capacidad, decimal tarifaPorNoche)
    {
        ValidateId(id);
        ValidateId(idPiso);
        ValidateId(idTipoHabitacion);

        ValidateRoomNumber(numero);
        ValidateCapacity(capacidad);
        ValidateRate(tarifaPorNoche);

        var bedroom = await _bedroomRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("La habitación solicitada no existe.");

        await EnsureValidFloorAsync(idPiso, bedroom.IdHotel, requireActive: idPiso != bedroom.IdPiso);

        await EnsureBedroomTypeExistsAsync(idTipoHabitacion);

        await EnsureUniqueRoomNumberAsync(bedroom.IdHotel, numero, id);

        bedroom.IdPiso = idPiso;
        bedroom.IdTipoHabitacion = idTipoHabitacion;
        bedroom.Numero = numero;
        bedroom.Capacidad = capacidad;
        bedroom.TarifaPorNoche = tarifaPorNoche;

        await _bedroomRepository.UpdateAsync(bedroom);

        return bedroom;
    }

    public async Task<Bedroom> ChangeStateAsync(int id, BedroomState estado)
    {
        ValidateId(id);

        if (!Enum.IsDefined(estado))
            throw new ArgumentException("El estado de la habitación no es válido.");

        var bedroom = await _bedroomRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("La habitación solicitada no existe.");

        if (estado == BedroomState.Disponible)
        {
            await EnsureValidFloorAsync(bedroom.IdPiso, bedroom.IdHotel, requireActive: true);
        }

        bedroom.Estado = estado;

        await _bedroomRepository.UpdateAsync(bedroom);

        return bedroom;
    }

    //Validaciones
    private async Task EnsureHotelExistsAsync(int idHotel)
    {
        var hotel = await _hotelRepository.GetByIdAsync(idHotel);

        if (hotel is null)
            throw new KeyNotFoundException("El hotel solicitado no existe.");
    }

    private async Task EnsureValidFloorAsync(int idPiso, int idHotel, bool requireActive)
    {
        var floor = await _floorRepository.GetByIdAsync(idPiso) ?? throw new KeyNotFoundException("El piso solicitado no existe.");

        if (floor.IdHotel != idHotel)
            throw new InvalidOperationException("El piso no pertenece al hotel indicado.");

        if (requireActive && floor.Estado != GeneralState.Activo)
            throw new InvalidOperationException("No se pueden utilizar habitaciones de pisos inactivos.");
    }

    private async Task EnsureBedroomTypeExistsAsync(int idTipoHabitacion)
    {
        var bedroomType = await _bedroomTypeRepository.GetByIdAsync(idTipoHabitacion);

        if (bedroomType is null)
            throw new KeyNotFoundException("El tipo de habitación solicitado no existe.");
    }

    private async Task EnsureUniqueRoomNumberAsync(int idHotel, short numero, int? excludeId = null)
    {
        var exists = await _bedroomRepository.ExistsByHotelAndNumberAsync(idHotel, numero, excludeId);

        if (exists)
            throw new InvalidOperationException("Ya existe una habitación con ese número en este hotel.");
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id),"El identificador debe ser mayor que cero.");
    }

    private static void ValidateRoomNumber(short numero)
    {
        if (numero <= 0)
            throw new ArgumentOutOfRangeException(nameof(numero),"El número de habitación debe ser mayor que cero.");
    }

    private static void ValidateCapacity(short capacidad)
    {
        if (capacidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacidad),"La capacidad debe ser mayor que cero.");
    }

    private static void ValidateRate(decimal tarifa)
    {
        if (tarifa < 0 || tarifa > 9999999999.99m)
            throw new ArgumentOutOfRangeException(nameof(tarifa),"La tarifa está fuera del rango permitido.");

        if (tarifa != decimal.Round(tarifa, 2))
            throw new ArgumentException("La tarifa solo puede tener dos decimales.");
    }
}