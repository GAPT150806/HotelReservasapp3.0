using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservasapp.DataAccess.Repositories;

public class BedroomTypeRepository : BaseRepository<BedroomType>, IBedroomTypeRepository
{
    public BedroomTypeRepository(HotelReservationDbContext context) : base(context)
    {
    }

    public async Task<BedroomType?> GetByNameAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Nombre == name);
    }
}