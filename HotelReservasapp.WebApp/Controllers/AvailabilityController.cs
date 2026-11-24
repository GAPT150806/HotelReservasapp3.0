
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using HotelReservasapp_.WebApp.Models.ViewModels.Availability;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservasapp_.WebApp.Controllers;

public class AvailabilityController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IHotelService _hotelService;
    private readonly IFloorService _floorService;
    private readonly IBedroomTypeService _bedroomTypeService;

    public AvailabilityController(IReservationService reservationService, IHotelService hotelService, IFloorService floorService, IBedroomTypeService bedroomTypeService)
    {
        _reservationService = reservationService;
        _hotelService = hotelService;
        _floorService = floorService;
        _bedroomTypeService = bedroomTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new AvailabilityViewModel();

        await LoadHotelsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AvailabilityViewModel model)
    {
        await LoadHotelsAsync(model);

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var bedrooms = await _reservationService.GetAvailableAsync(model.IdHotel, model.FechaEntrada!.Value, model.FechaSalida!.Value, model.CantidadHuespedes);

            var floors = await _floorService.GetByHotelAsync(model.IdHotel);
            var types = await _bedroomTypeService.GetAllAsync();
            var hotel = await _hotelService.GetByIdAsync(model.IdHotel);

            var floorLookup = floors.ToDictionary(f => f.Id, f => f.Numero);
            var typeLookup = types.ToDictionary(t => t.Id);

            model.NombreHotel = hotel?.Nombre ?? string.Empty;

            model.Habitaciones = bedrooms.Select(b =>
            {
                var tipo = typeLookup.GetValueOrDefault(b.IdTipoHabitacion);

                return new AvailableBedroomViewModel
                {
                    Id = b.Id,
                    Numero = b.Numero,
                    NumeroPiso = floorLookup.GetValueOrDefault(b.IdPiso),
                    TipoHabitacion = tipo?.Nombre ?? "Sin tipo",
                    Caracteristicas = tipo?.Caracteristicas,
                    Capacidad = b.Capacidad,
                    TarifaPorNoche = b.TarifaPorNoche,
                    TotalEstimado = b.TarifaPorNoche * model.Noches
                };
            }).ToList();

            model.BusquedaRealizada = true;
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(nameof(model.IdHotel), ex.Message);
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

    private async Task LoadHotelsAsync(AvailabilityViewModel model)
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
