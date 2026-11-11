using HotelReservasapp.BusinessLogic.Entities;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetAllAsync();

    Task<Reservation?> GetByIdAsync(int id);

    Task<IEnumerable<Bedroom>> GetAvailableAsync(int idHotel, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes);

    Task<Reservation> CreateAsync(int idHabitacion, int idHuesped, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes);

    Task<Reservation> ConfirmAsync(int id);

    Task<Reservation> ModifyAsync(int id, int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes);

    Task<Reservation> CancelAsync(int id);
}