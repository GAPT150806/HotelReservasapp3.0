
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BedroomTypesController : ControllerBase
{
    private readonly IBedroomTypeService _bedroomTypeService;

    public BedroomTypesController(IBedroomTypeService bedroomTypeService)
    {
        _bedroomTypeService = bedroomTypeService;
    }

    // GET: api/bedroomtypes
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var types = await _bedroomTypeService.GetAllAsync();

        return Ok(types.Select(t => new
        {
            t.Id,
            t.Nombre,
            t.Caracteristicas
        }));
    }

    // GET: api/bedroomtypes/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var type = await _bedroomTypeService.GetByIdAsync(id);

            if (type is null)
                return NotFound(new { message = "Tipo de habitación no encontrado." });

            return Ok(new
            {
                type.Id,
                type.Nombre,
                type.Caracteristicas
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/bedroomtypes
    [HttpPost]
    public async Task<IActionResult> Create(BedroomTypeRequest request)
    {
        try
        {
            var type = await _bedroomTypeService.CreateAsync(request.Nombre, request.Caracteristicas);

            return CreatedAtAction(nameof(GetById), new { id = type.Id }, new
                {
                    type.Id,
                    type.Nombre,
                    type.Caracteristicas
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/bedroomtypes/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BedroomTypeRequest request)
    {
        try
        {
            var type = await _bedroomTypeService.UpdateAsync(id, request.Nombre, request.Caracteristicas);

            return Ok(new
            {
                type.Id,
                type.Nombre,
                type.Caracteristicas
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}

public class BedroomTypeRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Caracteristicas { get; set; }
}
