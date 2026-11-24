
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservasapp_.WebApp.Models.ViewModels.Reservations;

public class ReservationViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una habitación.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una habitación válida.")]
    [Display(Name = "Habitación")]
    public int IdHabitacion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un huésped.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un huésped válido.")]
    [Display(Name = "Huésped")]
    public int IdHuesped { get; set; }

    [Required(ErrorMessage = "La fecha de entrada es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de entrada")]
    public DateTime FechaEntrada { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "La fecha de salida es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de salida")]
    public DateTime FechaSalida { get; set; } = DateTime.Today.AddDays(1);

    [Range(1, short.MaxValue, ErrorMessage = "Debe indicar al menos un huésped.")]
    [Display(Name = "Cantidad de huéspedes")]
    public short CantidadHuespedes { get; set; } = 1;

    public IEnumerable<SelectListItem> Huespedes { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> Habitaciones { get; set; } = Enumerable.Empty<SelectListItem>();
}


public class ReservationListItemViewModel
{
    public int Id { get; set; }

    public string Huesped { get; set; } = string.Empty;

    public short NumeroHabitacion { get; set; }

    public DateTime FechaEntrada { get; set; }

    public DateTime FechaSalida { get; set; }

    public short CantidadHuespedes { get; set; }

    public decimal TarifaPorNoche { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = string.Empty;
}
