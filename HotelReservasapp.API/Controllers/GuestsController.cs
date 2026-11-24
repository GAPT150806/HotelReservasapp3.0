
using HotelReservasapp.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;

    public GuestsController(IGuestService guestService)
    {
        _guestService = guestService;
    }

    // GET: api/guests
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var guests = await _guestService.GetAllAsync();

        return Ok(guests.Select(g => new
        {
            g.Id,
            g.Nombre,
            g.Apellido,
            g.Telefono,
            g.Correo
        }));
    }

    // GET: api/guests/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var guest = await _guestService.GetByIdAsync(id);

            if (guest is null)
                return NotFound(new { message = "Huésped no encontrado." });

            return Ok(new
            {
                guest.Id,
                guest.Nombre,
                guest.Apellido,
                guest.Telefono,
                guest.Correo
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/guests/by-email?correo=correo@ejemplo.com
    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string correo)
    {
        try
        {
            var guests = await _guestService.GetByCorreoAsync(correo);

            return Ok(guests.Select(g => new
            {
                g.Id,
                g.Nombre,
                g.Apellido,
                g.Telefono,
                g.Correo
            }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/guests/by-phone?telefono=8095551234
    [HttpGet("by-phone")]
    public async Task<IActionResult> GetByPhone([FromQuery] string telefono)
    {
        try
        {
            var guests = await _guestService.GetByTelefonoAsync(telefono);

            return Ok(guests.Select(g => new
            {
                g.Id,
                g.Nombre,
                g.Apellido,
                g.Telefono,
                g.Correo
            }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/guests
    [HttpPost]
    public async Task<IActionResult> Create(GuestRequest request)
    {
        try
        {
            var guest = await _guestService.CreateAsync(request.Nombre, request.Apellido, request.Telefono, request.Correo);

            return CreatedAtAction(nameof(GetById), new { id = guest.Id }, new
                {
                    guest.Id,
                    guest.Nombre,
                    guest.Apellido,
                    guest.Telefono,
                    guest.Correo
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/guests/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, GuestRequest request)
    {
        try
        {
            var guest = await _guestService.UpdateAsync(
                id,
                request.Nombre,
                request.Apellido,
                request.Telefono,
                request.Correo);

            return Ok(new
            {
                guest.Id,
                guest.Nombre,
                guest.Apellido,
                guest.Telefono,
                guest.Correo
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
    }
}

public class GuestRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
}
