
using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using HotelReservasapp_.WebApp.Models.ViewModels.Bedrooms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservasapp_.WebApp.Controllers;

public class BedroomsController : Controller
{
    private readonly IBedroomService _bedroomService;
    private readonly IHotelService _hotelService;
    private readonly IFloorService _floorService;
    private readonly IBedroomTypeService _bedroomTypeService;

    public BedroomsController(IBedroomService bedroomService, IHotelService hotelService, IFloorService floorService, IBedroomTypeService bedroomTypeService)
    {
        _bedroomService = bedroomService;
        _hotelService = hotelService;
        _floorService = floorService;
        _bedroomTypeService = bedroomTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var bedrooms = await _bedroomService.GetAllAsync();
        var hotels = await _hotelService.GetAllAsync();
        var floors = await _floorService.GetAllAsync();
        var types = await _bedroomTypeService.GetAllAsync();

        var hotelNames = hotels.ToDictionary(h => h.Id, h => h.Nombre);
        var floorNumbers = floors.ToDictionary(f => f.Id, f => f.Numero);
        var typeNames = types.ToDictionary(t => t.Id, t => t.Nombre);

        var model = bedrooms.Select(b => new BedroomViewModel
        {
            Id = b.Id,
            IdHotel = b.IdHotel,
            IdPiso = b.IdPiso,
            IdTipoHabitacion = b.IdTipoHabitacion,
            Numero = b.Numero,
            Capacidad = b.Capacidad,
            TarifaPorNoche = b.TarifaPorNoche,
            Estado = b.Estado,

            HotelNombre = hotelNames.TryGetValue(b.IdHotel, out var hotelName) ? hotelName : "Hotel no encontrado",

            PisoNombre = floorNumbers.TryGetValue(b.IdPiso, out var floorNumber) ? $"Piso {floorNumber}" : "Piso no encontrado",

            TipoNombre = typeNames.TryGetValue(b.IdTipoHabitacion, out var typeName) ? typeName : "Tipo no encontrado"
        });

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new BedroomViewModel();

        await LoadCatalogsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BedroomViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCatalogsAsync(model);
            return View(model);
        }

        try
        {
            await _bedroomService.CreateAsync(model.IdHotel, model.IdPiso, model.IdTipoHabitacion, model.Numero, model.Capacidad, model.TarifaPorNoche);

            TempData["SuccessMessage"] = "Habitación registrada correctamente.";

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

        await LoadCatalogsAsync(model);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return BadRequest();

        var bedroom = await _bedroomService.GetByIdAsync(id);

        if (bedroom is null)
            return NotFound();

        var hotel = await _hotelService.GetByIdAsync(bedroom.IdHotel);

        var model = new BedroomViewModel
        {
            Id = bedroom.Id,
            IdHotel = bedroom.IdHotel,
            HotelNombre = hotel?.Nombre ?? "Hotel no encontrado",
            IdPiso = bedroom.IdPiso,
            IdTipoHabitacion = bedroom.IdTipoHabitacion,
            Numero = bedroom.Numero,
            Capacidad = bedroom.Capacidad,
            TarifaPorNoche = bedroom.TarifaPorNoche,
            Estado = bedroom.Estado
        };

        await LoadCatalogsAsync(model, bedroom.IdPiso);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BedroomViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            var currentBedroom = await _bedroomService.GetByIdAsync(id);

            if (currentBedroom is null)
                return NotFound();

            model.IdHotel = currentBedroom.IdHotel;
            var hotel = await _hotelService.GetByIdAsync(currentBedroom.IdHotel);

            model.HotelNombre = hotel?.Nombre ?? "Hotel no encontrado";
            await LoadCatalogsAsync(model, currentBedroom.IdPiso);

            return View(model);
        }

        try
        {
            await _bedroomService.UpdateAsync(id, model.IdPiso, model.IdTipoHabitacion, model.Numero, model.Capacidad, model.TarifaPorNoche);

            TempData["SuccessMessage"] = "Habitación actualizada correctamente.";

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
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        var bedroom = await _bedroomService.GetByIdAsync(id);

        if (bedroom is null)
            return NotFound();

        model.IdHotel = bedroom.IdHotel;
        await LoadCatalogsAsync(model, bedroom.IdPiso);

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeState(int id, BedroomState estado)
    {
        if (id <= 0 || !Enum.IsDefined(estado))
        {
            TempData["ErrorMessage"] = "Los datos del cambio de estado no son válidos.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await _bedroomService.ChangeStateAsync(id, estado);

            TempData["SuccessMessage"] = "El estado de la habitación se actualizó correctamente.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
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


    private static string ObtenerNombreEstado(BedroomState estado)
    {
        return estado switch
        {
            BedroomState.Disponible => "Disponible",
            BedroomState.Reservada => "Reservada",
            BedroomState.Ocupada => "Ocupada",
            BedroomState.Limpieza => "Limpieza",
            BedroomState.Mantenimiento => "Mantenimiento",
            BedroomState.FueraServicio => "Fuera de servicio",
            _ => "Desconocido"
        };
    }


    [HttpGet]
    public async Task<IActionResult> GetFloorsByHotel(int idHotel)
    {
        if (idHotel <= 0)
            return BadRequest();

        var floors = await _floorService.GetByHotelAsync(idHotel);

        var result = floors.Where(f => f.Estado == GeneralState.Activo).Select(f => new
            {
                id = f.Id,
                nombre = $"Piso {f.Numero}"
            });

        return Json(result);
    }

    private async Task LoadCatalogsAsync(BedroomViewModel model, int? currentFloorId = null)
    {
        var hotels = await _hotelService.GetAllAsync();

        model.Hoteles = hotels.Where(h => h.Estado == GeneralState.Activo).Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Nombre,
                Selected = h.Id == model.IdHotel
            }).ToList();

        var types = await _bedroomTypeService.GetAllAsync();

        model.TiposHabitacion = types.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Nombre,
                Selected = t.Id == model.IdTipoHabitacion
            }).ToList();

        var selectedHotel = hotels.FirstOrDefault(h => h.Id == model.IdHotel);

        if (selectedHotel != null)
        {
            var floors = await _floorService.GetByHotelAsync(model.IdHotel);

            model.Pisos = floors.Where(f => f.Estado == GeneralState.Activo || f.Id == currentFloorId).Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = $"Piso {f.Numero}",
                    Selected = f.Id == model.IdPiso
                }).ToList();
        }
        else
        {
            model.Pisos = Enumerable.Empty<SelectListItem>();
        }
    }
}
