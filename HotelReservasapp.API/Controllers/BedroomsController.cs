
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BedroomsController : ControllerBase
{
    private readonly IBedroomService _bedroomService;

    public BedroomsController(IBedroomService bedroomService)
    {
        _bedroomService = bedroomService;
    }

    // GET: api/bedrooms
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bedrooms = await _bedroomService.GetAllAsync();

        return Ok(bedrooms.Select(b => new
        {
            b.Id,
            b.IdHotel,
            b.IdPiso,
            b.IdTipoHabitacion,
            b.Numero,
            b.Capacidad,
            b.TarifaPorNoche,
            Estado = b.Estado.ToString()
        }));
    }

    // GET: api/bedrooms/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var bedroom = await _bedroomService.GetByIdAsync(id);

            if (bedroom is null)
                return NotFound(new { message = "Habitación no encontrada." });

            return Ok(new
            {
                bedroom.Id,
                bedroom.IdHotel,
                bedroom.IdPiso,
                bedroom.IdTipoHabitacion,
                bedroom.Numero,
                bedroom.Capacidad,
                bedroom.TarifaPorNoche,
                Estado = bedroom.Estado.ToString()
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/bedrooms/by-hotel/26
    [HttpGet("by-hotel/{idHotel:int}")]
    public async Task<IActionResult> GetByHotel(int idHotel)
    {
        try
        {
            var bedrooms = await _bedroomService.GetByHotelAsync(idHotel);

            return Ok(bedrooms.Select(b => new
            {
                b.Id,
                b.IdHotel,
                b.IdPiso,
                b.IdTipoHabitacion,
                b.Numero,
                b.Capacidad,
                b.TarifaPorNoche,
                Estado = b.Estado.ToString()
            }));
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

    // GET: api/bedrooms/by-floor/1
    [HttpGet("by-floor/{idPiso:int}")]
    public async Task<IActionResult> GetByFloor(int idPiso)
    {
        try
        {
            var bedrooms = await _bedroomService.GetByFloorAsync(idPiso);

            return Ok(bedrooms.Select(b => new
            {
                b.Id,
                b.IdHotel,
                b.IdPiso,
                b.IdTipoHabitacion,
                b.Numero,
                b.Capacidad,
                b.TarifaPorNoche,
                Estado = b.Estado.ToString()
            }));
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

    // POST: api/bedrooms
    [HttpPost]
    public async Task<IActionResult> Create(BedroomRequest request)
    {
        try
        {
            var bedroom = await _bedroomService.CreateAsync(request.IdHotel, request.IdPiso, request.IdTipoHabitacion, request.Numero, request.Capacidad, request.TarifaPorNoche);

            return CreatedAtAction(nameof(GetById), new { id = bedroom.Id }, new
                {
                    bedroom.Id,
                    bedroom.IdHotel,
                    bedroom.IdPiso,
                    bedroom.IdTipoHabitacion,
                    bedroom.Numero,
                    bedroom.Capacidad,
                    bedroom.TarifaPorNoche,
                    Estado = bedroom.Estado.ToString()
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

    // PUT: api/bedrooms/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BedroomUpdateRequest request)
    {
        try
        {
            var bedroom = await _bedroomService.UpdateAsync(
                id,
                request.IdPiso,
                request.IdTipoHabitacion,
                request.Numero,
                request.Capacidad,
                request.TarifaPorNoche);

            return Ok(new
            {
                bedroom.Id,
                bedroom.IdHotel,
                bedroom.IdPiso,
                bedroom.IdTipoHabitacion,
                bedroom.Numero,
                bedroom.Capacidad,
                bedroom.TarifaPorNoche,
                Estado = bedroom.Estado.ToString()
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

    // PUT: api/bedrooms/1/state
    [HttpPut("{id:int}/state")]
    public async Task<IActionResult> ChangeState(int id, BedroomStateRequest request)
    {
        try
        {
            var bedroom = await _bedroomService.ChangeStateAsync(
                id,
                request.Estado);

            return Ok(new
            {
                bedroom.Id,
                bedroom.Numero,
                Estado = bedroom.Estado.ToString()
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

public class BedroomRequest
{
    public int IdHotel { get; set; }
    public int IdPiso { get; set; }
    public int IdTipoHabitacion { get; set; }
    public short Numero { get; set; }
    public short Capacidad { get; set; }
    public decimal TarifaPorNoche { get; set; }
}

public class BedroomUpdateRequest
{
    public int IdPiso { get; set; }
    public int IdTipoHabitacion { get; set; }
    public short Numero { get; set; }
    public short Capacidad { get; set; }
    public decimal TarifaPorNoche { get; set; }
}

public class BedroomStateRequest
{
    public BedroomState Estado { get; set; }
}
