
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    // GET: api/hotels
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var hotels = await _hotelService.GetAllAsync();

        return Ok(hotels.Select(h => new
        {
            h.Id,
            h.Nombre,
            h.Descripcion,
            h.Telefono,
            h.Direccion,
            Estado = h.Estado.ToString()
        }));
    }

    // GET: api/hotels/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var hotel = await _hotelService.GetByIdAsync(id);

            if (hotel is null)
                return NotFound(new { message = "Hotel no encontrado." });

            return Ok(new
            {
                hotel.Id,
                hotel.Nombre,
                hotel.Descripcion,
                hotel.Telefono,
                hotel.Direccion,
                Estado = hotel.Estado.ToString()
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/hotels
    [HttpPost]
    public async Task<IActionResult> Create(HotelRequest request)
    {
        try
        {
            var hotel = await _hotelService.CreateAsync(request.Nombre, request.Descripcion, request.Telefono, request.Direccion);

            return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, new
                {
                    hotel.Id,
                    hotel.Nombre,
                    hotel.Descripcion,
                    hotel.Telefono,
                    hotel.Direccion,
                    Estado = hotel.Estado.ToString()
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

    // PUT: api/hotels/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, HotelRequest request)
    {
        try
        {
            var hotel = await _hotelService.UpdateAsync(id, request.Nombre, request.Descripcion, request.Telefono, request.Direccion);

            return Ok(new
            {
                hotel.Id,
                hotel.Nombre,
                hotel.Descripcion,
                hotel.Telefono,
                hotel.Direccion,
                Estado = hotel.Estado.ToString()
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

    // PUT: api/hotels/1/state
    [HttpPut("{id:int}/state")]
    public async Task<IActionResult> ChangeState(int id, HotelStateRequest request)
    {
        try
        {
            var hotel = await _hotelService.ChangeStateAsync(
                id,
                request.Estado);

            return Ok(new
            {
                hotel.Id,
                hotel.Nombre,
                Estado = hotel.Estado.ToString()
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

public class HotelRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}

public class HotelStateRequest
{
    public GeneralState Estado { get; set; }
}
