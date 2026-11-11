using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Base;

namespace HotelReservasapp.BusinessLogic.Entities
{
    public sealed class Hotel : BaseEntity
    {
        public required string Nombre { get; set; }
        public GeneralState Estado { get; set; } = GeneralState.Activo;
    }
}
