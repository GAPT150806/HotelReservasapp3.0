using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.DataAccess.Data;
using HotelReservasapp.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservasapp.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SGRHotel") ?? throw new InvalidOperationException("No se encontró la conexión SGRHotel.");

        services.AddDbContext<HotelReservationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IHotelRepository, HotelRepository>();

        services.AddScoped<IFloorRepository, FloorRepository>();

        services.AddScoped<IBedroomTypeRepository, BedroomTypeRepository>();

        services.AddScoped<IBedroomRepository, BedroomRepository>();

        services.AddScoped<IGuestRepository, GuestRepository>();

        services.AddScoped<IReservationRepository, ReservationRepository>();

        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}