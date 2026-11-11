using HotelReservasapp.BusinessLogic.Entities;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface IBedroomTypeService
{
    Task<IEnumerable<BedroomType>> GetAllAsync();

    Task<BedroomType?> GetByIdAsync(int id);

    Task<BedroomType> CreateAsync(string nombre, string? caracteristicas);

    Task<BedroomType> UpdateAsync(int id, string nombre, string? caracteristicas);
}