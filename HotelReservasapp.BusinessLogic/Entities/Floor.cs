using HotelReservasapp.BusinessLogic.Base;
using HotelReservasapp.BusinessLogic.Enums;

namespace HotelReservasapp.BusinessLogic.Entities
{
    public sealed class Floor : BaseEntity
    {
        public int IdHotel { get; set; }
        public short Numero { get; set; }
        public GeneralState Estado { get; set; } = GeneralState.Activo;
    }
}
