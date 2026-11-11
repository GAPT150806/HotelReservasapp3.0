
using System.ComponentModel.DataAnnotations;
using HotelReservasapp.BusinessLogic.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelReservasapp_.WebApp.Models.ViewModels.Floors;

public class FloorViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un hotel.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un hotel válido.")]
    [Display(Name = "Hotel")]
    public int IdHotel { get; set; }

    public string HotelNombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número del piso es obligatorio.")]
    [Range(1, short.MaxValue, ErrorMessage = "El número debe ser mayor que cero.")]
    [Display(Name = "Número del piso")]
    public short Numero { get; set; }

    public GeneralState Estado { get; set; } = GeneralState.Activo;

    public IEnumerable<SelectListItem> Hoteles { get; set; } = Enumerable.Empty<SelectListItem>();
}
