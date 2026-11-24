
using HotelReservasapp.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public AvailabilityController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    // GET: api/availability
    [HttpGet]
    public async Task<IActionResult> GetAvailable([FromQuery] int idHotel, [FromQuery] DateTime fechaEntrada, [FromQuery] DateTime fechaSalida, [FromQuery] short cantidadHuespedes)
    {
        try
        {
            var bedrooms = await _reservationService.GetAvailableAsync(idHotel, fechaEntrada, fechaSalida, cantidadHuespedes);

            return Ok(bedrooms.Select(b => new
            {
                b.Id,
                b.IdHotel,
                b.IdPiso,
                b.IdTipoHabitacion,
                b.Numero,
                b.Capacidad,
                b.TarifaPorNoche,
                Noches = (fechaSalida.Date - fechaEntrada.Date).Days,
                TotalEstimado = b.TarifaPorNoche *
                    (fechaSalida.Date - fechaEntrada.Date).Days
            }));
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
}
