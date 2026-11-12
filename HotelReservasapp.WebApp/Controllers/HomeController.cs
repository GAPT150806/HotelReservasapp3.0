
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using HotelReservasapp_.WebApp.Models;
using HotelReservasapp_.WebApp.Models.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelReservasapp_.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IHotelService _hotelService;
    private readonly IFloorService _floorService;
    private readonly IBedroomService _bedroomService;
    private readonly IBedroomTypeService _bedroomTypeService;

    public HomeController(IHotelService hotelService, IFloorService floorService, IBedroomService bedroomService, IBedroomTypeService bedroomTypeService)
    {
        _hotelService = hotelService;
        _floorService = floorService;
        _bedroomService = bedroomService;
        _bedroomTypeService = bedroomTypeService;
    }

    public async Task<IActionResult> Index()
    {
        var hoteles = (await _hotelService.GetAllAsync()).ToList();
        var pisos = (await _floorService.GetAllAsync()).ToList();
        var habitaciones = (await _bedroomService.GetAllAsync()).ToList();
        var tipos = (await _bedroomTypeService.GetAllAsync()).ToList();

        int totalHabitaciones = habitaciones.Count;

        var estados = new List<BedroomStateSummaryViewModel>
        {
            CrearEstado("Disponible", habitaciones.Count(h => h.Estado == BedroomState.Disponible), totalHabitaciones, "available"),

            CrearEstado("Reservada", habitaciones.Count(h => h.Estado == BedroomState.Reservada), totalHabitaciones, "reserved"),

            CrearEstado("Ocupada", habitaciones.Count(h => h.Estado == BedroomState.Ocupada), totalHabitaciones, "occupied"),

            CrearEstado("Limpieza", habitaciones.Count(h => h.Estado == BedroomState.Limpieza), totalHabitaciones, "cleaning"),

            CrearEstado("Mantenimiento", habitaciones.Count(h => h.Estado == BedroomState.Mantenimiento), totalHabitaciones, "maintenance"),

            CrearEstado("Fuera de servicio", habitaciones.Count(h => h.Estado == BedroomState.FueraServicio), totalHabitaciones, "out-of-service")
        };

        var model = new DashboardViewModel
        {
            TotalHoteles = hoteles.Count, HotelesActivos = hoteles.Count(h => h.Estado == GeneralState.Activo),

            TotalPisos = pisos.Count, PisosActivos = pisos.Count(p => p.Estado == GeneralState.Activo),

            TotalHabitaciones = totalHabitaciones, TotalTiposHabitacion = tipos.Count,

            EstadosHabitaciones = estados
        };

        return View(model);
    }

    private static BedroomStateSummaryViewModel CrearEstado(string nombre, int cantidad, int total, string cssClass)
    {
        decimal porcentaje = total == 0 ? 0 : Math.Round(cantidad * 100m / total, 1);

        return new BedroomStateSummaryViewModel
        {
            Nombre = nombre,
            Cantidad = cantidad,
            Porcentaje = porcentaje,
            CssClass = cssClass
        };
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
