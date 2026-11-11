using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Interfaces;

namespace HotelReservasapp.BusinessLogic.Services;

public sealed class BedroomTypeService : IBedroomTypeService
{
    private readonly IBedroomTypeRepository _bedroomTypeRepository;

    public BedroomTypeService(IBedroomTypeRepository bedroomTypeRepository)
    {
        _bedroomTypeRepository = bedroomTypeRepository ?? throw new ArgumentNullException(nameof(bedroomTypeRepository));
    }

    public async Task<IEnumerable<BedroomType>> GetAllAsync()
    {
        return await _bedroomTypeRepository.GetAllAsync();
    }

    public async Task<BedroomType?> GetByIdAsync(int id)
    {
        ValidateId(id);

        return await _bedroomTypeRepository.GetByIdAsync(id);
    }

    public async Task<BedroomType> CreateAsync(string nombre, string? caracteristicas)
    {
        nombre = ValidateName(nombre);

        caracteristicas = ValidateCharacteristics(caracteristicas);

        var existingType = await _bedroomTypeRepository.GetByNameAsync(nombre);

        if (existingType is not null)
            throw new InvalidOperationException("Ya existe un tipo de habitación con ese nombre.");

        var bedroomType = new BedroomType
        {
            Nombre = nombre,
            Caracteristicas = caracteristicas
        };

        await _bedroomTypeRepository.AddAsync(bedroomType);

        return bedroomType;
    }

    public async Task<BedroomType> UpdateAsync(int id, string nombre, string? caracteristicas)
    {
        ValidateId(id);

        nombre = ValidateName(nombre);

        caracteristicas = ValidateCharacteristics(caracteristicas);

        var bedroomType = await _bedroomTypeRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("El tipo de habitación solicitado no existe.");

        var existingType = await _bedroomTypeRepository.GetByNameAsync(nombre);

        if (existingType is not null && existingType.Id != id)
            throw new InvalidOperationException("Ya existe otro tipo de habitación con ese nombre.");

        bedroomType.Nombre = nombre;
        bedroomType.Caracteristicas = caracteristicas;

        await _bedroomTypeRepository.UpdateAsync(bedroomType);

        return bedroomType;
    }

    //Validaciones
    private static string ValidateName(string nombre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        nombre = nombre.Trim();

        if (nombre.Length > 100)
            throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

        return nombre;
    }

    private static string? ValidateCharacteristics(string? caracteristicas)
    {
        if (string.IsNullOrWhiteSpace(caracteristicas))
            return null;

        caracteristicas = caracteristicas.Trim();

        if (caracteristicas.Length > 500)
            throw new ArgumentException("Las características no pueden superar los 500 caracteres.");

        return caracteristicas;
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id),"El identificador debe ser mayor que cero.");
    }
}