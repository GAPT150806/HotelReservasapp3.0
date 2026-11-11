using HotelReservasapp.BusinessLogic.Base;
using HotelReservasapp.BusinessLogic.Enums;

namespace HotelReservasapp.BusinessLogic.Entities
{
    public sealed class Notification : BaseEntity
    {
        public int IdReserva { get; set; }
        public NotificacionType Tipo { get; set; }
        public NotificationState Estado { get; set; } = NotificationState.Pendiente;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        
    }
}
