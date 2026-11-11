using HotelReservasapp.BusinessLogic.Entities;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface IReservationRepository : IBaseRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByHabitacionAsync(int idHabitacion);

    Task<bool> HasActiveOverlapAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida, int? excludingReservationId = null);

    Task AddIfNoActiveOverlapAsync(Reservation reservation);

    Task UpdateIfNoActiveOverlapAsync(Reservation reservation);
}