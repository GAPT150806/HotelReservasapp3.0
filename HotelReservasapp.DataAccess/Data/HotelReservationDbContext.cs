using HotelReservasapp.BusinessLogic.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelReservasapp.DataAccess.Data
{
    public class HotelReservationDbContext : DbContext
    {
        public HotelReservationDbContext(DbContextOptions<HotelReservationDbContext> options) : base(options)
        {
        }

        public DbSet<Hotel> Hoteles => Set<Hotel>();
        public DbSet<Floor> Pisos => Set<Floor>();
        public DbSet<Reservation> Reservas => Set<Reservation>();
        public DbSet<Bedroom> Habitaciones => Set<Bedroom>();
        public DbSet<BedroomType> TiposHabitacion => Set<BedroomType>();
        public DbSet<Guest> Huespedes => Set<Guest>();
        public DbSet<Notification> Notificaciones => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HotelReservationDbContext).Assembly);
        }
    }
}
