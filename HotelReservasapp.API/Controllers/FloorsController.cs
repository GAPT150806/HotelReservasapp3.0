
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FloorsController : ControllerBase
{
    private readonly IFloorService _floorService;

    public FloorsController(IFloorService floorService)
    {
        _floorService = floorService;
    }

    // GET: api/floors
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var floors = await _floorService.GetAllAsync();

        return Ok(floors.Select(f => new
        {
            f.Id,
            f.IdHotel,
            f.Numero,
            Estado = f.Estado.ToString()
        }));
    }

    // GET: api/floors/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var floor = await _floorService.GetByIdAsync(id);

            if (floor is null)
                return NotFound(new { message = "Piso no encontrado." });

            return Ok(new
            {
                floor.Id,
                floor.IdHotel,
                floor.Numero,
                Estado = floor.Estado.ToString()
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/floors/by-hotel/26
    [HttpGet("by-hotel/{idHotel:int}")]
    public async Task<IActionResult> GetByHotel(int idHotel)
    {
        try
        {
            var floors = await _floorService.GetByHotelAsync(idHotel);

            return Ok(floors.Select(f => new
            {
                f.Id,
                f.IdHotel,
                f.Numero,
                Estado = f.Estado.ToString()
            }));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // POST: api/floors
    [HttpPost]
    public async Task<IActionResult> Create(FloorRequest request)
    {
        try
        {
            var floor = await _floorService.CreateAsync(request.IdHotel, request.Numero);

            return CreatedAtAction(nameof(GetById), new { id = floor.Id }, new
                {
                    floor.Id,
                    floor.IdHotel,
                    floor.Numero,
                    Estado = floor.Estado.ToString()
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/floors/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FloorUpdateRequest request)
    {
        try
        {
            var floor = await _floorService.UpdateAsync(
                id,
                request.Numero);

            return Ok(new
            {
                floor.Id,
                floor.IdHotel,
                floor.Numero,
                Estado = floor.Estado.ToString()
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

    // PUT: api/floors/1/state
    [HttpPut("{id:int}/state")]
    public async Task<IActionResult> ChangeState(int id, FloorStateRequest request)
    {
        try
        {
            var floor = await _floorService.ChangeStateAsync(
                id,
                request.Estado);

            return Ok(new
            {
                floor.Id,
                floor.IdHotel,
                floor.Numero,
                Estado = floor.Estado.ToString()
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

public class FloorRequest
{
    public int IdHotel { get; set; }
    public short Numero { get; set; }
}

public class FloorUpdateRequest
{
    public short Numero { get; set; }
}

public class FloorStateRequest
{
    public GeneralState Estado { get; set; }
}
