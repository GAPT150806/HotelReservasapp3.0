
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp_.WebApp.Models.ViewModels.Hotels;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp_.WebApp.Controllers;

public class HotelsController : Controller
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var hotels = await _hotelService.GetAllAsync();

        var model = hotels.Select(h => new HotelViewModel
        {
            Id = h.Id,
            Nombre = h.Nombre,
            Estado = h.Estado
        });

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new HotelViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HotelViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _hotelService.CreateAsync(model.Nombre);

            TempData["SuccessMessage"] = "Hotel registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(model.Nombre), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.Nombre), ex.Message);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return BadRequest();

        var hotel = await _hotelService.GetByIdAsync(id);

        if (hotel is null)
            return NotFound();

        var model = new HotelViewModel
        {
            Id = hotel.Id,
            Nombre = hotel.Nombre
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HotelViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _hotelService.UpdateAsync(id, model.Nombre);

            TempData["SuccessMessage"] = "Hotel actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(model.Nombre), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.Nombre), ex.Message);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeState(int id, GeneralState estado)
    {
        if (id <= 0 || !Enum.IsDefined(estado))
            return BadRequest();

        try
        {
            await _hotelService.ChangeStateAsync(id, estado);

            TempData["SuccessMessage"] = estado == GeneralState.Activo ? "Hotel activado correctamente." : "Hotel desactivado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
