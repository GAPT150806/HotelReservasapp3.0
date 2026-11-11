using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservasapp.DataAccess.Repositories;

public class FloorRepository : BaseRepository<Floor>, IFloorRepository
{
    public FloorRepository(HotelReservationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Floor>> GetByHotelAsync(int idHotel)
    {
        if (idHotel <= 0)
            throw new ArgumentOutOfRangeException(nameof(idHotel));

        return await _dbSet.AsNoTracking().Where(p => p.IdHotel == idHotel).ToListAsync();
    }

    public async Task<bool> ExistsActiveByHotelAndNumberAsync(int idHotel, short numero, int? excludeId = null)
    {
        if (idHotel <= 0)
            throw new ArgumentOutOfRangeException(nameof(idHotel));

        if (numero <= 0)
            throw new ArgumentOutOfRangeException(nameof(numero));

        var query = _dbSet.Where(p => p.IdHotel == idHotel && p.Numero == numero && p.Estado == GeneralState.Activo);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}