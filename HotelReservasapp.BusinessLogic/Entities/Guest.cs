using HotelReservasapp.BusinessLogic.Base;

namespace HotelReservasapp.BusinessLogic.Entities
{
    public sealed class Guest : BaseEntity
    {
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
    }
}
