using HotelReservasapp.BusinessLogic.Entities;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface IGuestService
{
    Task<IEnumerable<Guest>> GetAllAsync();

    Task<Guest?> GetByIdAsync(int id);

    Task<IEnumerable<Guest>> GetByCorreoAsync(string correo);

    Task<IEnumerable<Guest>> GetByTelefonoAsync(string telefono);

    Task<Guest> CreateAsync(string nombre, string apellido, string? telefono, string? correo);

    Task<Guest> UpdateAsync(int id, string nombre, string apellido, string? telefono, string? correo);
}