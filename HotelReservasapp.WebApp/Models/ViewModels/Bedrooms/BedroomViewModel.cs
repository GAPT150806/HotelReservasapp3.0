
using System.ComponentModel.DataAnnotations;
using HotelReservasapp.BusinessLogic.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservasapp_.WebApp.Models.ViewModels.Bedrooms;

public class BedroomViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un hotel.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un hotel válido.")]
    [Display(Name = "Hotel")]
    public int IdHotel { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un piso.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un piso válido.")]
    [Display(Name = "Piso")]
    public int IdPiso { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de habitación.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo válido.")]
    [Display(Name = "Tipo de habitación")]
    public int IdTipoHabitacion { get; set; }

    [Required(ErrorMessage = "El número es obligatorio.")]
    [Range(1, short.MaxValue, ErrorMessage = "El número debe ser mayor que cero.")]
    [Display(Name = "Número de habitación")]
    public short Numero { get; set; }

    [Required(ErrorMessage = "La capacidad es obligatoria.")]
    [Range(1, short.MaxValue, ErrorMessage = "La capacidad debe ser mayor que cero.")]
    [Display(Name = "Capacidad")]
    public short Capacidad { get; set; }

    [Required(ErrorMessage = "La tarifa es obligatoria.")]
    [Range(typeof(decimal), "0", "9999999999.99",
        ErrorMessage = "La tarifa debe estar entre 0 y 9,999,999,999.99.")]
    [Display(Name = "Tarifa por noche")]
    public decimal TarifaPorNoche { get; set; }

    public BedroomState Estado { get; set; }

    public string HotelNombre { get; set; } = string.Empty;
    public string PisoNombre { get; set; } = string.Empty;
    public string TipoNombre { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> Hoteles { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> Pisos { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> TiposHabitacion { get; set; } = Enumerable.Empty<SelectListItem>();
}
