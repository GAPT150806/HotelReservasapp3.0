
using System.ComponentModel.DataAnnotations;

namespace HotelReservasapp_.WebApp.Models.ViewModels.Guests;

public class GuestViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [StringLength(25)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "Introduzca un correo electrónico válido.")]
    [StringLength(254)]
    [Display(Name = "Correo electrónico")]
    public string? Correo { get; set; }
}
