
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservasapp_.WebApp.Models.ViewModels.Availability;

public class AvailabilityViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un hotel.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un hotel válido.")]
    [Display(Name = "Hotel")]
    public int IdHotel { get; set; }

    [Required(ErrorMessage = "La fecha de entrada es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de entrada")]
    public DateTime? FechaEntrada { get; set; }

    [Required(ErrorMessage = "La fecha de salida es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de salida")]
    public DateTime? FechaSalida { get; set; }

    [Required(ErrorMessage = "Indique la cantidad de huéspedes.")]
    [Range(1, short.MaxValue, ErrorMessage = "Debe indicar al menos un huésped.")]
    [Display(Name = "Cantidad de huéspedes")]
    public short CantidadHuespedes { get; set; } = 1;

    public IEnumerable<SelectListItem> Hoteles { get; set; } = Enumerable.Empty<SelectListItem>();

    public List<AvailableBedroomViewModel> Habitaciones { get; set; } = [];

    public bool BusquedaRealizada { get; set; }

    public string NombreHotel { get; set; } = string.Empty;

    public int Noches => FechaEntrada.HasValue && FechaSalida.HasValue ? (FechaSalida.Value.Date - FechaEntrada.Value.Date).Days : 0;
}

public class AvailableBedroomViewModel
{
    public int Id { get; set; }

    public short Numero { get; set; }

    public short NumeroPiso { get; set; }

    public string TipoHabitacion { get; set; } = string.Empty;

    public string? Caracteristicas { get; set; }

    public short Capacidad { get; set; }

    public decimal TarifaPorNoche { get; set; }

    public decimal TotalEstimado { get; set; }
}
