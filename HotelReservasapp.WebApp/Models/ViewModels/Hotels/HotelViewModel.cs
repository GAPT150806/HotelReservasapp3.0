
using HotelReservasapp.BusinessLogic.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelReservasapp_.WebApp.Models.ViewModels.Hotels;

public class HotelViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del hotel es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    [Display(Name = "Nombre del hotel")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [StringLength(25, ErrorMessage = "El teléfono no puede superar los 25 caracteres.")]
    [Display(Name = "Número de teléfono")]
    public string? Telefono { get; set; }

    [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
    [Display(Name = "Dirección física")]
    public string? Direccion { get; set; }

    public GeneralState Estado { get; set; } = GeneralState.Activo;

}
