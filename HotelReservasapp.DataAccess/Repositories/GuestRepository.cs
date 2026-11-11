using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservasapp.DataAccess.Repositories;

public class GuestRepository : BaseRepository<Guest>, IGuestRepository
{
    public GuestRepository(HotelReservationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Guest>> GetByCorreo(string correo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correo);

        return await _dbSet.AsNoTracking().Where(h => h.Correo == correo).ToListAsync();
    }

    public async Task<IEnumerable<Guest>> GetByTelefono(string telefono)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(telefono);

        return await _dbSet.AsNoTracking().Where(h => h.Telefono == telefono).ToListAsync();
    }
}