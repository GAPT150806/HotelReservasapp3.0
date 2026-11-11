
using System.ComponentModel.DataAnnotations;

namespace HotelReservasapp_.WebApp.Models.ViewModels.BedroomTypes;

public class BedroomTypeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    [Display(Name = "Nombre del tipo")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Las características no pueden superar los 500 caracteres.")]
    [Display(Name = "Características")]
    public string? Caracteristicas { get; set; }
}
