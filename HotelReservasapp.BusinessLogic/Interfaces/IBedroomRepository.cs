using HotelReservasapp.BusinessLogic.Entities;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface IBedroomRepository : IBaseRepository<Bedroom>
{
    Task<IEnumerable<Bedroom>> GetByHotelAsync(int idHotel);

    Task<IEnumerable<Bedroom>> GetByPisoAsync(int idPiso);

    Task<IEnumerable<Bedroom>> GetAvailableAsync(int idHotel, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes, int? excludingReservationId = null);

    Task<bool> ExistsByHotelAndNumberAsync(int idHotel, short numero, int? excludeId = null);
}