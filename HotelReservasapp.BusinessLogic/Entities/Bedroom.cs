
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Base;

namespace HotelReservasapp.BusinessLogic.Entities
{
    public sealed class Bedroom : BaseEntity
    {
        public int IdHotel { get; set; }
        public int IdPiso { get; set; }
        public int IdTipoHabitacion { get; set; }
        public short Numero { get; set; }
        public short Capacidad { get; set; }
        public decimal TarifaPorNoche { get; set; }
        public BedroomState Estado { get; set; } = BedroomState.Disponible;
    }
}
