using HotelReservasapp.BusinessLogic.Base;

namespace HotelReservasapp.BusinessLogic.Entities
{
    public sealed class BedroomType : BaseEntity
    {
        public required string Nombre { get; set; }
        public string? Caracteristicas { get; set; }
    }
}
