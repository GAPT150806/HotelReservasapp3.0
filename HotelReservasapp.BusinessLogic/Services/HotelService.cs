using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;

namespace HotelReservasapp.BusinessLogic.Services;

public sealed class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;

    public HotelService(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository
            ?? throw new ArgumentNullException(nameof(hotelRepository));
    }

    public async Task<IEnumerable<Hotel>> GetAllAsync()
    {
        return await _hotelRepository.GetAllAsync();
    }

    public async Task<Hotel?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await _hotelRepository.GetByIdAsync(id);
    }

    public async Task<Hotel> CreateAsync(string nombre)
    {
        nombre = ValidateName(nombre);

        var existingHotel =
            await _hotelRepository.GetByNameAsync(nombre);

        if (existingHotel is not null)
            throw new InvalidOperationException("Ya existe un hotel con ese nombre.");

        var hotel = new Hotel
        {
            Nombre = nombre,
            Estado = GeneralState.Activo
        };

        await _hotelRepository.AddAsync(hotel);

        return hotel;
    }

    public async Task<Hotel> UpdateAsync(int id, string nombre)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        nombre = ValidateName(nombre);

        var hotel = await _hotelRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("El hotel solicitado no existe.");

        var existingHotel = await _hotelRepository.GetByNameAsync(nombre);

        if (existingHotel is not null && existingHotel.Id != id)
            throw new InvalidOperationException("Ya existe otro hotel con ese nombre.");

        hotel.Nombre = nombre;

        await _hotelRepository.UpdateAsync(hotel);

        return hotel;
    }

    public async Task<Hotel> ChangeStateAsync(int id, GeneralState estado)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        if (!Enum.IsDefined(estado))
            throw new ArgumentException("Estado inválido.");

        var hotel = await _hotelRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("El hotel solicitado no existe.");

        hotel.Estado = estado;

        await _hotelRepository.UpdateAsync(hotel);

        return hotel;
    }


    //Validaciones
    private static string ValidateName(string nombre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        nombre = nombre.Trim();

        if (nombre.Length > 150)
            throw new ArgumentException("El nombre no puede superar los 150 caracteres.");

        return nombre;
    }
}