
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
    public GeneralState Estado { get; set; } = GeneralState.Activo;

}
