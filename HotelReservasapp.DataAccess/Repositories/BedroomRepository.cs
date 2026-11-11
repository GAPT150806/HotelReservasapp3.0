using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelReservasapp.DataAccess.Repositories;

public class BedroomRepository : BaseRepository<Bedroom>, IBedroomRepository
{
    public BedroomRepository(HotelReservationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Bedroom>> GetByHotelAsync(int idHotel)
    {
        if (idHotel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(idHotel));
        }
            
        return await _dbSet.AsNoTracking().Where(h => h.IdHotel == idHotel).ToListAsync();
    }

    public async Task<IEnumerable<Bedroom>> GetByPisoAsync(int idPiso)
    {
        if (idPiso <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(idPiso));
        }
           
        return await _dbSet.AsNoTracking().Where(h => h.IdPiso == idPiso).ToListAsync();
    }

    public async Task<IEnumerable<Bedroom>> GetAvailableAsync(int idHotel, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes, int? excludingReservationId = null)
    {
        if (idHotel <= 0) throw new ArgumentOutOfRangeException(nameof(idHotel));
        if (fechaSalida <= fechaEntrada) throw new ArgumentException("La fecha de salida debe ser posterior a la entrada.");
        if (cantidadHuespedes <= 0) throw new ArgumentOutOfRangeException(nameof(cantidadHuespedes));

        return await (
            from bedroom in _context.Habitaciones.AsNoTracking()
            join floor in _context.Pisos.AsNoTracking() on new { bedroom.IdPiso, bedroom.IdHotel } equals new { IdPiso = floor.Id, floor.IdHotel }
            join hotel in _context.Hoteles.AsNoTracking() on bedroom.IdHotel equals hotel.Id
            where bedroom.IdHotel == idHotel
                && hotel.Estado == GeneralState.Activo
                && floor.Estado == GeneralState.Activo
                && bedroom.Estado == BedroomState.Disponible
                && bedroom.Capacidad >= cantidadHuespedes
                && !_context.Reservas.Any(reservation =>
                    reservation.IdHabitacion == bedroom.Id
                    && (reservation.Estado == ReservaState.Pendiente
                        || reservation.Estado == ReservaState.Confirmada
                        || reservation.Estado == ReservaState.En_Curso)
                    && (!excludingReservationId.HasValue || reservation.Id != excludingReservationId.Value)
                    && reservation.FechaEntrada < fechaSalida
                    && reservation.FechaSalida > fechaEntrada)
            select bedroom
        ).ToListAsync();
    }

    public async Task<bool> ExistsByHotelAndNumberAsync(int idHotel, short numero, int? excludeId = null)
    {
        if (idHotel <= 0)
            throw new ArgumentOutOfRangeException(nameof(idHotel));

        if (numero <= 0)
            throw new ArgumentOutOfRangeException(nameof(numero));

        var query = _dbSet.AsNoTracking().Where(b => b.IdHotel == idHotel && b.Numero == numero);

        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);

        return await query.AnyAsync();
    }
}
