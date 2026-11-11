
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using HotelReservasapp_.WebApp.Models.ViewModels.Floors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservasapp_.WebApp.Controllers;

public class FloorsController : Controller
{
    private readonly IFloorService _floorService;
    private readonly IHotelService _hotelService;

    public FloorsController(IFloorService floorService, IHotelService hotelService)
    {
        _floorService = floorService;
        _hotelService = hotelService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var floors = await _floorService.GetAllAsync();
        var hotels = await _hotelService.GetAllAsync();

        var hotelNames = hotels.ToDictionary(h => h.Id, h => h.Nombre);

        var model = floors.Select(f => new FloorViewModel
        {
            Id = f.Id,
            IdHotel = f.IdHotel,
            HotelNombre = hotelNames.TryGetValue(f.IdHotel, out var name) ? name : "Hotel no encontrado",
            Numero = f.Numero,
            Estado = f.Estado
        });

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new FloorViewModel();

        await LoadHotelsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FloorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadHotelsAsync(model);
            return View(model);
        }

        try
        {
            await _floorService.CreateAsync(model.IdHotel, model.Numero);

            TempData["SuccessMessage"] = "Piso registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(nameof(model.IdHotel), ex.Message);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(model.Numero), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.Numero), ex.Message);
        }

        await LoadHotelsAsync(model);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return BadRequest();

        var floor = await _floorService.GetByIdAsync(id);

        if (floor is null)
            return NotFound();

        var hotel = await _hotelService.GetByIdAsync(floor.IdHotel);

        var model = new FloorViewModel
        {
            Id = floor.Id,
            IdHotel = floor.IdHotel,
            HotelNombre = hotel?.Nombre ?? "Hotel no encontrado",
            Numero = floor.Numero,
            Estado = floor.Estado
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FloorViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            var floor = await _floorService.GetByIdAsync(id);

            if (floor is null)
                return NotFound();

            var hotel = await _hotelService.GetByIdAsync(floor.IdHotel);
            model.HotelNombre = hotel?.Nombre ?? "Hotel no encontrado";

            return View(model);
        }

        try
        {
            await _floorService.UpdateAsync(id, model.Numero);

            TempData["SuccessMessage"] = "Piso actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(model.Numero), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.Numero), ex.Message);
        }

        var currentFloor = await _floorService.GetByIdAsync(id);

        if (currentFloor is null)
            return NotFound();

        var currentHotel = await _hotelService.GetByIdAsync(currentFloor.IdHotel);
        model.HotelNombre = currentHotel?.Nombre ?? "Hotel no encontrado";

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
            await _floorService.ChangeStateAsync(id, estado);

            TempData["SuccessMessage"] = estado == GeneralState.Activo ? "Piso activado correctamente." : "Piso desactivado correctamente.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadHotelsAsync(FloorViewModel model)
    {
        var hotels = await _hotelService.GetAllAsync();

        model.Hoteles = hotels.Where(h => h.Estado == GeneralState.Activo).Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Nombre,
                Selected = h.Id == model.IdHotel
            }).ToList();
    }
}
