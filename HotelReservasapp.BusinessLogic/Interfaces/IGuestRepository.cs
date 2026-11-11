using HotelReservasapp.BusinessLogic.Entities;

namespace HotelReservasapp.BusinessLogic.Interfaces
{
    public interface IGuestRepository : IBaseRepository<Guest>
    {
        Task<IEnumerable<Guest>> GetByCorreo(string correo);
        Task<IEnumerable<Guest>> GetByTelefono(string telefono);
    }
}
