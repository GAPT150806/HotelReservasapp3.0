using HotelReservasapp.BusinessLogic.Entities;

namespace HotelReservasapp.BusinessLogic.Interfaces
{
    public interface IBedroomTypeRepository : IBaseRepository<BedroomType>
    {
        Task<BedroomType?> GetByNameAsync(string name);
    }
}
