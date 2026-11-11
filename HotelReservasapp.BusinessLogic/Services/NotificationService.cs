using HotelReservasapp.BusinessLogic.Entities;
using HotelReservasapp.BusinessLogic.Enums;
using HotelReservasapp.BusinessLogic.Interfaces;

namespace HotelReservasapp.BusinessLogic.Services;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IReservationRepository _reservationRepository;

    public NotificationService(INotificationRepository notificationRepository, IReservationRepository reservationRepository)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));

        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        ValidateId(id);

        return await _notificationRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetByReservationAsync(int idReserva)
    {
        ValidateId(idReserva);

        await GetRequiredReservationAsync(idReserva);

        return await _notificationRepository.GetByReservationAsync(idReserva);
    }

    public async Task<Notification> RegisterAsync(int idReserva, NotificacionType tipo)
    {
        ValidateId(idReserva);

        if (!Enum.IsDefined(tipo))
            throw new ArgumentException("El tipo de notificación no es válido.");

        var reservation = await GetRequiredReservationAsync(idReserva);

        ValidateNotificationEvent(reservation, tipo);

        var notification = new Notification
        {
            IdReserva = idReserva,
            Tipo = tipo,
            Estado = NotificationState.Pendiente,
            FechaEnvio = null
            // FechaCreacion la genera SQL Server.
        };

        await _notificationRepository.AddAsync(notification);

        return notification;
    }

    public async Task<Notification> MarkAsSentAsync(int id)
    {
        ValidateId(id);

        var notification = await GetRequiredNotificationAsync(id);

        if (notification.Estado == NotificationState.Enviada)
            return notification;

        if (notification.Estado != NotificationState.Pendiente)
            throw new InvalidOperationException("Solamente se pueden enviar notificaciones pendientes.");

        notification.Estado = NotificationState.Enviada;
        notification.FechaEnvio = DateTime.Now;

        await _notificationRepository.UpdateAsync(notification);

        return notification;
    }

    public async Task<Notification> MarkAsFailedAsync(int id)
    {
        ValidateId(id);

        var notification = await GetRequiredNotificationAsync(id);

        if (notification.Estado == NotificationState.Fallida)
            return notification;

        if (notification.Estado != NotificationState.Pendiente)
            throw new InvalidOperationException("Solamente pueden marcarse como fallidas las notificaciones pendientes.");

        notification.Estado = NotificationState.Fallida;
        notification.FechaEnvio = null;

        await _notificationRepository.UpdateAsync(notification);

        return notification;
    }

    public async Task<Notification> RetryAsync(int id)
    {
        ValidateId(id);

        var notification = await GetRequiredNotificationAsync(id);

        if (notification.Estado == NotificationState.Pendiente)
            return notification;

        if (notification.Estado != NotificationState.Fallida)
            throw new InvalidOperationException("Solamente pueden reintentarse las notificaciones fallidas.");

        notification.Estado = NotificationState.Pendiente;
        notification.FechaEnvio = null;

        await _notificationRepository.UpdateAsync(notification);

        return notification;
    }

    private async Task<Reservation> GetRequiredReservationAsync(int idReserva)
    {
        return await _reservationRepository.GetByIdAsync(idReserva) ?? throw new KeyNotFoundException("La reserva asociada no existe.");
    }

    private async Task<Notification> GetRequiredNotificationAsync(int id)
    {
        return await _notificationRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("La notificación solicitada no existe.");
    }

    //Validaciones
    private static void ValidateNotificationEvent(Reservation reservation, NotificacionType tipo)
    {
        switch (tipo)
        {
            case NotificacionType.Confirmacion:

                if (reservation.Estado != ReservaState.Confirmada)
                    throw new InvalidOperationException("La reserva debe estar confirmada para generar esta notificación.");

                break;

            case NotificacionType.Modificacion:

                if (reservation.Estado != ReservaState.Pendiente &&
                    reservation.Estado != ReservaState.Confirmada)
                {
                    throw new InvalidOperationException("La reserva no tiene un estado válido para notificar su modificación.");
                }

                break;

            case NotificacionType.Cancelacion:

                if (reservation.Estado != ReservaState.Cancelada)
                    throw new InvalidOperationException("La reserva debe estar cancelada para generar esta notificación.");

                break;

            default:

                throw new ArgumentException("Tipo de notificación no reconocido.");
        }
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id),"El identificador debe ser mayor que cero.");
    }
}