
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Base;

namespace HotelReservasapp.BusinessLogic.Entities
{
    public sealed class Hotel : BaseEntity
    {
        public required string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public GeneralState Estado { get; set; } = GeneralState.Activo;
    }
}
