using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;

namespace HotelReservasapp.BusinessLogic.Interfaces.Services;

public interface IFloorService
{
    Task<IEnumerable<Floor>> GetAllAsync();

    Task<Floor?> GetByIdAsync(int id);

    Task<IEnumerable<Floor>> GetByHotelAsync(int idHotel);

    Task<Floor> CreateAsync(int idHotel, short numero);

    Task<Floor> UpdateAsync(int id, short numero);

    Task<Floor> ChangeStateAsync(int id, GeneralState estado);
}