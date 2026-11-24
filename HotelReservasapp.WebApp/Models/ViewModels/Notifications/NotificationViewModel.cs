
using HotelReservasapp.BusinessLogic.Enums;

namespace HotelReservasapp_.WebApp.Models.ViewModels.Notifications;

public class NotificationViewModel
{
    public int Id { get; set; }

    public int IdReserva { get; set; }

    public string Huesped { get; set; } = string.Empty;

    public NotificacionType Tipo { get; set; }

    public NotificationState Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaEnvio { get; set; }
}
