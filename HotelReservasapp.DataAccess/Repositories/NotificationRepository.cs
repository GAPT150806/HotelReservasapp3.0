using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservasapp.DataAccess.Repositories;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(HotelReservationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notification>> GetByReservationAsync(int idReserva)
    {
        if (idReserva <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(idReserva));
        }

        return await _dbSet.AsNoTracking().Where(n => n.IdReserva == idReserva).OrderByDescending(n => n.FechaCreacion).ToListAsync();
    }
}