using HotelReservasapp.BusinessLogic.Entities;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface IFloorRepository : IBaseRepository<Floor>
{
    Task<IEnumerable<Floor>> GetByHotelAsync(int idHotel);

    Task<bool> ExistsActiveByHotelAndNumberAsync(int idHotel, short numero, int? excludeId = null);
}