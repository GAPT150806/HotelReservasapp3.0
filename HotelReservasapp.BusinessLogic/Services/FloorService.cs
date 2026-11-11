using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;

namespace HotelReservasapp.BusinessLogic.Services;

public sealed class FloorService : IFloorService
{
    private readonly IFloorRepository _floorRepository;
    private readonly IHotelRepository _hotelRepository;

    public FloorService(IFloorRepository floorRepository, IHotelRepository hotelRepository)
    {
        _floorRepository = floorRepository ?? throw new ArgumentNullException(nameof(floorRepository));

        _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
    }

    public async Task<IEnumerable<Floor>> GetAllAsync()
    {
        return await _floorRepository.GetAllAsync();
    }

    public async Task<Floor?> GetByIdAsync(int id)
    {
        ValidateId(id);

        return await _floorRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Floor>> GetByHotelAsync(int idHotel)
    {
        ValidateId(idHotel);

        await EnsureHotelExistsAsync(idHotel);

        return await _floorRepository.GetByHotelAsync(idHotel);
    }

    public async Task<Floor> CreateAsync(int idHotel, short numero)
    {
        ValidateId(idHotel);
        ValidateNumber(numero);

        await EnsureHotelExistsAsync(idHotel);

        await EnsureNoDuplicateActiveFloorAsync(idHotel, numero);

        var floor = new Floor
        {
            IdHotel = idHotel,
            Numero = numero,
            Estado = GeneralState.Activo
        };

        await _floorRepository.AddAsync(floor);

        return floor;
    }

    public async Task<Floor> UpdateAsync(int id, short numero)
    {
        ValidateId(id);
        ValidateNumber(numero);

        var floor = await _floorRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("El piso solicitado no existe.");

        if (floor.Estado == GeneralState.Activo)
        {
            await EnsureNoDuplicateActiveFloorAsync(floor.IdHotel, numero, id);
        }

        floor.Numero = numero;

        await _floorRepository.UpdateAsync(floor);

        return floor;
    }

    public async Task<Floor> ChangeStateAsync(int id, GeneralState estado)
    {
        ValidateId(id);

        if (!Enum.IsDefined(estado))
            throw new ArgumentException("Estado inválido.");

        var floor = await _floorRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("El piso solicitado no existe.");

        if (estado == GeneralState.Activo &&
            floor.Estado != GeneralState.Activo)
        {
            await EnsureNoDuplicateActiveFloorAsync(floor.IdHotel, floor.Numero, id);
        }

        floor.Estado = estado;

        await _floorRepository.UpdateAsync(floor);

        return floor;
    }

    //Validaciones
    private async Task EnsureHotelExistsAsync(int idHotel)
    {
        var hotel = await _hotelRepository.GetByIdAsync(idHotel);

        if (hotel is null)
            throw new KeyNotFoundException("El hotel al que pertenece el piso no existe.");
    }

    private async Task EnsureNoDuplicateActiveFloorAsync(int idHotel, short numero, int? excludeId = null)
    {
        var exists = await _floorRepository.ExistsActiveByHotelAndNumberAsync( idHotel, numero, excludeId);

        if (exists)
            throw new InvalidOperationException("Ya existe un piso activo con ese número en este hotel.");
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));
    }

    private static void ValidateNumber(short numero)
    {
        if (numero <= 0)
            throw new ArgumentOutOfRangeException(nameof(numero), "El número del piso debe ser mayor que cero.");
    }
}