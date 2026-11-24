
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp_.WebApp.Models.ViewModels.Reservations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservasapp_.WebApp.Controllers;

public class ReservationsController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IGuestService _guestService;
    private readonly IBedroomService _bedroomService;

    public ReservationsController(IReservationService reservationService, IGuestService guestService, IBedroomService bedroomService)
    {
        _reservationService = reservationService;
        _guestService = guestService;
        _bedroomService = bedroomService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida, short cantidadHuespedes)
    {
        var bedroom = await _bedroomService.GetByIdAsync(idHabitacion);

        if (bedroom is null)
            return NotFound();

        var model = new ReservationViewModel
        {
            IdHabitacion = idHabitacion,
            FechaEntrada = fechaEntrada,
            FechaSalida = fechaSalida,
            CantidadHuespedes = cantidadHuespedes
        };

        await LoadGuestsAsync(model);

        ViewBag.Habitacion = bedroom.Numero;
        ViewBag.Capacidad = bedroom.Capacidad;
        ViewBag.Tarifa = bedroom.TarifaPorNoche;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationViewModel model)
    {
        await LoadGuestsAsync(model);

        if (!ModelState.IsValid)
        {
            var selectedBedroom =
                await _bedroomService.GetByIdAsync(model.IdHabitacion);

            if (selectedBedroom is not null)
            {
                ViewBag.Habitacion = selectedBedroom.Numero;
                ViewBag.Capacidad = selectedBedroom.Capacidad;
                ViewBag.Tarifa = selectedBedroom.TarifaPorNoche;
            }

            return View(model);
        }

        try
        {
            var reservation = await _reservationService.CreateAsync(model.IdHabitacion, model.IdHuesped, model.FechaEntrada, model.FechaSalida, model.CantidadHuespedes);

            TempData["SuccessMessage"] = $"Reserva #{reservation.Id} registrada correctamente. Total: {reservation.Total:C}";

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        var bedroom = await _bedroomService.GetByIdAsync(model.IdHabitacion);

        if (bedroom is not null)
        {
            ViewBag.Habitacion = bedroom.Numero;
            ViewBag.Capacidad = bedroom.Capacidad;
            ViewBag.Tarifa = bedroom.TarifaPorNoche;
        }

        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var reservations = await _reservationService.GetAllAsync();
        var guests = await _guestService.GetAllAsync();
        var bedrooms = await _bedroomService.GetAllAsync();

        var guestLookup = guests.ToDictionary(g => g.Id);
        var bedroomLookup = bedrooms.ToDictionary(b => b.Id);

        var model = reservations.Select(r => new ReservationListItemViewModel
        {
            Id = r.Id,

            Huesped = guestLookup.TryGetValue(r.IdHuesped, out var guest) ? $"{guest.Nombre} {guest.Apellido}" : "Huésped no encontrado",

            NumeroHabitacion = bedroomLookup.TryGetValue(r.IdHabitacion, out var bedroom) ? bedroom.Numero : (short)0,
            FechaEntrada = r.FechaEntrada,
            FechaSalida = r.FechaSalida,
            CantidadHuespedes = r.CantidadHuespedes,
            TarifaPorNoche = r.TarifaPorNoche,
            Total = r.Total,
            Estado = r.Estado.ToString()
        }).ToList();

        return View(model);
    }



    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);

        if (reservation is null)
            return NotFound();

        var model = new ReservationViewModel
        {
            Id = reservation.Id,
            IdHabitacion = reservation.IdHabitacion,
            IdHuesped = reservation.IdHuesped,
            FechaEntrada = reservation.FechaEntrada,
            FechaSalida = reservation.FechaSalida,
            CantidadHuespedes = reservation.CantidadHuespedes
        };

        await LoadGuestsAsync(model);
        await LoadBedroomsAsync(model);

        return View(model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReservationViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        await LoadGuestsAsync(model);
        await LoadBedroomsAsync(model);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _reservationService.ModifyAsync(id, model.IdHabitacion, model.IdHuesped, model.FechaEntrada, model.FechaSalida, model.CantidadHuespedes);

            TempData["SuccessMessage"] = "Reserva modificada correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int id)
    {
        try
        {
            await _reservationService.ConfirmAsync(id);
            TempData["SuccessMessage"] = "Reserva confirmada correctamente.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _reservationService.CancelAsync(id);
            TempData["SuccessMessage"] = "Reserva cancelada correctamente.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadBedroomsAsync(ReservationViewModel model)
    {
        var bedrooms = await _bedroomService.GetAllAsync();

        model.Habitaciones = bedrooms.Select(b => new SelectListItem
        {
            Value = b.Id.ToString(),
            Text = $"Habitación {b.Numero}",
            Selected = b.Id == model.IdHabitacion
        }).ToList();
    }

    private async Task LoadGuestsAsync(ReservationViewModel model)
    {
        var guests = await _guestService.GetAllAsync();

        model.Huespedes = guests.Select(g => new SelectListItem
        {
            Value = g.Id.ToString(),
            Text = $"{g.Nombre} {g.Apellido}",
            Selected = g.Id == model.IdHuesped
        }).ToList();
    }
}
