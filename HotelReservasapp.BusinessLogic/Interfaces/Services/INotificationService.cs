using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;

namespace HotelReservasapp.BusinessLogic.Interfaces;

public interface INotificationService
{
    Task<Notification?> GetByIdAsync(int id);

    Task<IEnumerable<Notification>> GetByReservationAsync(int idReserva);

    Task<Notification> RegisterAsync(int idReserva, NotificacionType tipo);

    Task<Notification> MarkAsSentAsync(int id);

    Task<Notification> MarkAsFailedAsync(int id);

    Task<Notification> RetryAsync(int id);
}