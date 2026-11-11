using System.Data;
using System.Globalization;
using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HotelReservasapp.DataAccess.Repositories;

public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(HotelReservationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reservation>> GetByHabitacionAsync(int idHabitacion)
    {
        ValidateRoomId(idHabitacion);

        return await _dbSet.AsNoTracking().Where(r => r.IdHabitacion == idHabitacion).ToListAsync();
    }

    public async Task<bool> HasActiveOverlapAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida, int? excludingReservationId = null)
    {
        ValidateRoomId(idHabitacion);
        ValidateDates(fechaEntrada, fechaSalida);

        return await HasActiveOverlapInternalAsync(idHabitacion, fechaEntrada, fechaSalida, excludingReservationId);
    }

    public async Task AddIfNoActiveOverlapAsync(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        ValidateReservation(reservation);

        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            await AcquireApplicationLockAsync($"HotelReservasapp:Habitacion:{reservation.IdHabitacion}");

            var hasOverlap = await HasActiveOverlapInternalAsync(reservation.IdHabitacion, reservation.FechaEntrada, reservation.FechaSalida);

            if (hasOverlap)
                throw new InvalidOperationException("La habitación ya tiene una reserva activa para ese período.");

            await _dbSet.AddAsync(reservation);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            _context.Entry(reservation).State = EntityState.Detached;

            throw;
        }
    }

    public async Task UpdateIfNoActiveOverlapAsync(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        ValidateReservation(reservation);

        if (reservation.Id <= 0)
            throw new ArgumentOutOfRangeException(nameof(reservation));

        var entry = _context.Entry(reservation);

        if (entry.State == EntityState.Detached)
            throw new InvalidOperationException("La reserva debe estar siendo rastreada por el contexto.");

        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        try
        {
            await AcquireApplicationLockAsync($"HotelReservasapp:Reserva:{reservation.Id}");

            await AcquireApplicationLockAsync($"HotelReservasapp:Habitacion:{reservation.IdHabitacion}");

            var databaseValues = await entry.GetDatabaseValuesAsync();

            if (databaseValues is null)
                throw new KeyNotFoundException("La reserva ya no existe.");

            foreach (var property in entry.Metadata.GetProperties())
            {
                if (!Equals(databaseValues[property.Name], entry.OriginalValues[property.Name]))
                {
                    throw new DbUpdateConcurrencyException("La reserva fue modificada por otra operación. " + "Vuelve a consultar sus datos.");
                }
            }

            var blocksAvailability = reservation.Estado is ReservaState.Pendiente or ReservaState.Confirmada or ReservaState.En_Curso;

            if (blocksAvailability)
            {
                var hasOverlap = await HasActiveOverlapInternalAsync(reservation.IdHabitacion, reservation.FechaEntrada, reservation.FechaSalida, reservation.Id);

                if (hasOverlap)
                    throw new InvalidOperationException("La habitación ya tiene una reserva activa para ese período.");
            }

            entry.State = EntityState.Modified;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            if (entry.State != EntityState.Detached)
                await entry.ReloadAsync();

            throw;
        }
    }

    private async Task<bool> HasActiveOverlapInternalAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida, int? excludingReservationId = null)
    {
        var activeStates = new[]
        {
            ReservaState.Pendiente,
            ReservaState.Confirmada,
            ReservaState.En_Curso
        };

        var query = _dbSet.AsNoTracking().Where(r => r.IdHabitacion == idHabitacion && activeStates.Contains(r.Estado) && r.FechaEntrada < fechaSalida && r.FechaSalida > fechaEntrada);

        if (excludingReservationId.HasValue)
        {
            query = query.Where(r => r.Id != excludingReservationId.Value);
        }

        return await query.AnyAsync();
    }

    private async Task AcquireApplicationLockAsync(string resource)
    {
        var currentTransaction = _context.Database.CurrentTransaction ?? throw new InvalidOperationException("No existe una transacción activa.");

        await using var command = _context.Database.GetDbConnection().CreateCommand();

        command.Transaction = currentTransaction.GetDbTransaction();

        command.CommandText = """
            DECLARE @result int;

            EXEC @result = sys.sp_getapplock
                @Resource = @resource,
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = 15000;

            SELECT @result;
            """;

        var parameter = command.CreateParameter();

        parameter.ParameterName = "@resource";
        parameter.DbType = DbType.String;
        parameter.Size = 255;
        parameter.Value = resource;

        command.Parameters.Add(parameter);

        var resultValue = await command.ExecuteScalarAsync();

        if (resultValue is null || resultValue == DBNull.Value)
            throw new InvalidOperationException("SQL Server no devolvió el resultado del bloqueo.");

        var result = Convert.ToInt32(resultValue, CultureInfo.InvariantCulture);

        if (result < 0)
            throw new InvalidOperationException($"No se pudo obtener el bloqueo de SQL Server. Código: {result}.");
    }

    //Validaciones

    private static void ValidateReservation(Reservation reservation)
    {
        ValidateRoomId(reservation.IdHabitacion);

        ValidateDates(reservation.FechaEntrada, reservation.FechaSalida);

        if (reservation.CantidadHuespedes <= 0)
            throw new ArgumentOutOfRangeException(nameof(reservation.CantidadHuespedes));
    }

    private static void ValidateRoomId(int idHabitacion)
    {
        if (idHabitacion <= 0)
            throw new ArgumentOutOfRangeException(nameof(idHabitacion));
    }

    private static void ValidateDates(DateTime fechaEntrada, DateTime fechaSalida)
    {
        if (fechaSalida <= fechaEntrada)
            throw new ArgumentException("La fecha de salida debe ser posterior a la entrada.");
    }
}