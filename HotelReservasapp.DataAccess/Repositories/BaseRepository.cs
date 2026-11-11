
using HotelReservasapp.BusinessLogic.Base;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservasapp.DataAccess.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly HotelReservationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(HotelReservationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _dbSet = _context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id),"El ID debe ser mayor que cero.");

        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(entity);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (entity.Id <= 0)
            throw new ArgumentException("La entidad debe tener un ID válido.", nameof(entity));

        var entry = _context.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
    }
}
