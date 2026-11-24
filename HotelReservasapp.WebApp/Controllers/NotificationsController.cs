
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp_.WebApp.Models.ViewModels.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp_.WebApp.Controllers;

public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly IReservationService _reservationService;
    private readonly IGuestService _guestService;

    public NotificationsController(INotificationService notificationService, IReservationService reservationService, IGuestService guestService)
    {
        _notificationService = notificationService;
        _reservationService = reservationService;
        _guestService = guestService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int idReserva)
    {
        if (idReserva <= 0)
            return BadRequest();

        var reservation = await _reservationService.GetByIdAsync(idReserva);

        if (reservation is null)
            return NotFound();

        var guest = await _guestService.GetByIdAsync(reservation.IdHuesped);

        var notifications =
            await _notificationService.GetByReservationAsync(idReserva);

        var model = notifications.Select(n => new NotificationViewModel
        {
            Id = n.Id,
            IdReserva = n.IdReserva,
            Huesped = guest is null ? "Huésped no encontrado" : $"{guest.Nombre} {guest.Apellido}",
            Tipo = n.Tipo,
            Estado = n.Estado,
            FechaCreacion = n.FechaCreacion,
            FechaEnvio = n.FechaEnvio
        }).ToList();

        ViewBag.IdReserva = idReserva;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsSent(int id, int idReserva)
    {
        try
        {
            var notification = await _notificationService.MarkAsSentAsync(id);
            TempData["Success"] = "Notificación marcada como enviada.";
            return RedirectToAction(nameof(Index), new { idReserva = notification.IdReserva });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException or ArgumentException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index), new { idReserva });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsFailed(int id, int idReserva)
    {
        try
        {
            var notification = await _notificationService.MarkAsFailedAsync(id);
            TempData["Success"] = "Notificación marcada como fallida.";
            return RedirectToAction(nameof(Index), new { idReserva = notification.IdReserva });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException or ArgumentException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index), new { idReserva });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Retry(int id, int idReserva)
    {
        try
        {
            var notification = await _notificationService.RetryAsync(id);
            TempData["Success"] = "Notificación devuelta al estado pendiente.";
            return RedirectToAction(nameof(Index), new { idReserva = notification.IdReserva });
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException or ArgumentException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index), new { idReserva });
        }
    }

}
