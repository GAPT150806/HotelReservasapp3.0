using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Interfaces;

namespace HotelReservasapp.BusinessLogic.Services;

public sealed class GuestService : IGuestService
{
    private readonly IGuestRepository _guestRepository;

    public GuestService(IGuestRepository guestRepository)
    {
        _guestRepository = guestRepository ?? throw new ArgumentNullException(nameof(guestRepository));
    }

    public async Task<IEnumerable<Guest>> GetAllAsync()
    {
        return await _guestRepository.GetAllAsync();
    }

    public async Task<Guest?> GetByIdAsync(int id)
    {
        ValidateId(id);

        return await _guestRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Guest>> GetByCorreoAsync(string correo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(correo);

        return await _guestRepository.GetByCorreo(correo.Trim());
    }

    public async Task<IEnumerable<Guest>> GetByTelefonoAsync(string telefono)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(telefono);

        return await _guestRepository.GetByTelefono(telefono.Trim());
    }

    public async Task<Guest> CreateAsync(string nombre, string apellido, string? telefono, string? correo)
    {
        nombre = ValidateRequired(nombre, 100, nameof(nombre));
        apellido = ValidateRequired(apellido, 100, nameof(apellido));

        telefono = ValidateOptional(telefono, 25, nameof(telefono));
        correo = ValidateOptional(correo, 254, nameof(correo));

        var guest = new Guest
        {
            Nombre = nombre,
            Apellido = apellido,
            Telefono = telefono,
            Correo = correo
        };

        await _guestRepository.AddAsync(guest);

        return guest;
    }

    public async Task<Guest> UpdateAsync(int id, string nombre, string apellido, string? telefono, string? correo)
    {
        ValidateId(id);

        nombre = ValidateRequired(nombre, 100, nameof(nombre));
        apellido = ValidateRequired(apellido, 100, nameof(apellido));

        telefono = ValidateOptional(telefono, 25, nameof(telefono));
        correo = ValidateOptional(correo, 254, nameof(correo));

        var guest = await _guestRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("El huésped solicitado no existe.");

        guest.Nombre = nombre;
        guest.Apellido = apellido;
        guest.Telefono = telefono;
        guest.Correo = correo;

        await _guestRepository.UpdateAsync(guest);

        return guest;
    }

    //Validaciones
    private static string ValidateRequired(string value, int maxLength, string fieldName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        value = value.Trim();

        if (value.Length > maxLength)
            throw new ArgumentException($"{fieldName} no puede superar los {maxLength} caracteres.");

        return value;
    }

    private static string? ValidateOptional(string? value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim();

        if (value.Length > maxLength)
            throw new ArgumentException($"{fieldName} no puede superar los {maxLength} caracteres.");

        return value;
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id),"El identificador debe ser mayor que cero.");
    }
}