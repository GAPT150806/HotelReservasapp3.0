
using HotelReservasapp.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // GET: api/notifications/6
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var notification = await _notificationService.GetByIdAsync(id);

            if (notification is null)
                return NotFound(new { message = "Notificación no encontrada." });

            return Ok(new
            {
                notification.Id,
                notification.IdReserva,
                Tipo = notification.Tipo.ToString(),
                Estado = notification.Estado.ToString(),
                notification.FechaCreacion,
                notification.FechaEnvio
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/notifications/by-reservation/1
    [HttpGet("by-reservation/{idReserva:int}")]
    public async Task<IActionResult> GetByReservation(int idReserva)
    {
        try
        {
            var notifications = await _notificationService.GetByReservationAsync(idReserva);

            return Ok(notifications.Select(n => new
            {
                n.Id,
                n.IdReserva,
                Tipo = n.Tipo.ToString(),
                Estado = n.Estado.ToString(),
                n.FechaCreacion,
                n.FechaEnvio
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
}
