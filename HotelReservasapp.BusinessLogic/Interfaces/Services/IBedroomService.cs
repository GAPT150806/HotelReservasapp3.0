using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface IBedroomService
{
    Task<IEnumerable<Bedroom>> GetAllAsync();

    Task<Bedroom?> GetByIdAsync(int id);

    Task<IEnumerable<Bedroom>> GetByHotelAsync(int idHotel);

    Task<IEnumerable<Bedroom>> GetByFloorAsync(int idPiso);

    Task<Bedroom> CreateAsync(int idHotel, int idPiso, int idTipoHabitacion, short numero, short capacidad, decimal tarifaPorNoche);

    Task<Bedroom> UpdateAsync(int id, int idPiso, int idTipoHabitacion, short numero, short capacidad, decimal tarifaPorNoche);

    Task<Bedroom> ChangeStateAsync(int id, BedroomState estado);
}