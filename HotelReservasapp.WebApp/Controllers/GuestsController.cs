
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using HotelReservasapp_.WebApp.Models.ViewModels.Guests;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp_.WebApp.Controllers;

public class GuestsController : Controller
{
    private readonly IGuestService _guestService;

    public GuestsController(IGuestService guestService)
    {
        _guestService = guestService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var guests = await _guestService.GetAllAsync();

        var model = guests.Select(g => new GuestViewModel
        {
            Id = g.Id,
            Nombre = g.Nombre,
            Apellido = g.Apellido,
            Telefono = g.Telefono,
            Correo = g.Correo
        });

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new GuestViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GuestViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _guestService.CreateAsync(model.Nombre, model.Apellido, model.Telefono, model.Correo);

            TempData["SuccessMessage"] = "Huésped registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return BadRequest();

        var guest = await _guestService.GetByIdAsync(id);

        if (guest is null)
            return NotFound();

        var model = new GuestViewModel
        {
            Id = guest.Id,
            Nombre = guest.Nombre,
            Apellido = guest.Apellido,
            Telefono = guest.Telefono,
            Correo = guest.Correo
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GuestViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _guestService.UpdateAsync(id, model.Nombre, model.Apellido, model.Telefono, model.Correo);

            TempData["SuccessMessage"] = "Huésped actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        return View(model);
    }
}
