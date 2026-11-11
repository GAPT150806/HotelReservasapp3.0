
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp_.WebApp.Models.ViewModels.BedroomTypes;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservasapp_.WebApp.Controllers;

public class BedroomTypesController : Controller
{
    private readonly IBedroomTypeService _bedroomTypeService;

    public BedroomTypesController(IBedroomTypeService bedroomTypeService)
    {
        _bedroomTypeService = bedroomTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var types = await _bedroomTypeService.GetAllAsync();

        var model = types.Select(t => new BedroomTypeViewModel
        {
            Id = t.Id,
            Nombre = t.Nombre,
            Caracteristicas = t.Caracteristicas
        });

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new BedroomTypeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BedroomTypeViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _bedroomTypeService.CreateAsync(
                model.Nombre,
                model.Caracteristicas);

            TempData["SuccessMessage"] = "Tipo de habitación registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
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

        var type = await _bedroomTypeService.GetByIdAsync(id);

        if (type is null)
            return NotFound();

        var model = new BedroomTypeViewModel
        {
            Id = type.Id,
            Nombre = type.Nombre,
            Caracteristicas = type.Caracteristicas
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BedroomTypeViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _bedroomTypeService.UpdateAsync(id, model.Nombre, model.Caracteristicas);

            TempData["SuccessMessage"] = "Tipo de habitación actualizado correctamente.";

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
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.Nombre), ex.Message);
        }

        return View(model);
    }
}
