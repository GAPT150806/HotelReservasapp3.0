
using HotelReservasapp.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    // GET: api/reservations
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reservations = await _reservationService.GetAllAsync();

        return Ok(reservations.Select(r => new
        {
            r.Id,
            r.IdHabitacion,
            r.IdHuesped,
            r.FechaEntrada,
            r.FechaSalida,
            r.CantidadHuespedes,
            r.TarifaPorNoche,
            r.Total,
            Estado = r.Estado.ToString()
        }));
    }

    // GET: api/reservations/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var reservation = await _reservationService.GetByIdAsync(id);

            if (reservation is null)
                return NotFound(new { message = "Reserva no encontrada." });

            return Ok(new
            {
                reservation.Id,
                reservation.IdHabitacion,
                reservation.IdHuesped,
                reservation.FechaEntrada,
                reservation.FechaSalida,
                reservation.CantidadHuespedes,
                reservation.TarifaPorNoche,
                reservation.Total,
                Estado = reservation.Estado.ToString()
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/reservations
    [HttpPost]
    public async Task<IActionResult> Create(ReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.CreateAsync(request.IdHabitacion, request.IdHuesped, request.FechaEntrada, request.FechaSalida, request.CantidadHuespedes);

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, new
                {
                    reservation.Id,
                    reservation.IdHabitacion,
                    reservation.IdHuesped,
                    reservation.FechaEntrada,
                    reservation.FechaSalida,
                    reservation.CantidadHuespedes,
                    reservation.TarifaPorNoche,
                    reservation.Total,
                    Estado = reservation.Estado.ToString()
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

    // PUT: api/reservations/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Modify(int id, ReservationRequest request)
    {
        try
        {
            var reservation = await _reservationService.ModifyAsync(
                id,
                request.IdHabitacion,
                request.IdHuesped,
                request.FechaEntrada,
                request.FechaSalida,
                request.CantidadHuespedes);

            return Ok(new
            {
                reservation.Id,
                reservation.IdHabitacion,
                reservation.IdHuesped,
                reservation.FechaEntrada,
                reservation.FechaSalida,
                reservation.CantidadHuespedes,
                reservation.TarifaPorNoche,
                reservation.Total,
                Estado = reservation.Estado.ToString()
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

    // PUT: api/reservations/1/confirm
    [HttpPut("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        try
        {
            var reservation = await _reservationService.ConfirmAsync(id);

            return Ok(new
            {
                reservation.Id,
                Estado = reservation.Estado.ToString()
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

    // PUT: api/reservations/1/cancel
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var reservation = await _reservationService.CancelAsync(id);

            return Ok(new
            {
                reservation.Id,
                Estado = reservation.Estado.ToString()
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

public class ReservationRequest
{
    public int IdHabitacion { get; set; }
    public int IdHuesped { get; set; }
    public DateTime FechaEntrada { get; set; }
    public DateTime FechaSalida { get; set; }
    public short CantidadHuespedes { get; set; }
}
