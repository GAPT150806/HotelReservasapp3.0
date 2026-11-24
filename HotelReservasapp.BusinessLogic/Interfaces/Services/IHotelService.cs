
using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface IHotelService
{
    Task<IEnumerable<Hotel>> GetAllAsync();

    Task<Hotel?> GetByIdAsync(int id);

    Task<Hotel> CreateAsync(string nombre, string? descripcion, string? telefono, string? direccion);

    Task<Hotel> UpdateAsync(int id, string nombre, string? descripcion, string? telefono, string? direccion);

    Task<Hotel> ChangeStateAsync(int id, GeneralState estado);
}
