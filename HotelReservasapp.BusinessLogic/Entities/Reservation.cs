using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Base;

namespace HotelReservasapp.BusinessLogic.Entities
{
    public sealed class Reservation : BaseEntity
    {
        public int IdHabitacion { get; set; }
        public int IdHuesped { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public short CantidadHuespedes { get; set; }
        public decimal TarifaPorNoche { get; set; }
        public decimal Total { get; set; }
        public ReservaState Estado { get; set; } = ReservaState.Pendiente;
    }
}
