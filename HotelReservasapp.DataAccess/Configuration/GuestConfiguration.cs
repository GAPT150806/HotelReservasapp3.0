using HotelReservasapp.BusinessLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservasapp.DataAccess.Configurations;

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.ToTable("Huespedes");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.Apellido)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.Telefono)
            .HasMaxLength(25)
            .IsRequired(false);

        builder.Property(h => h.Correo)
            .HasMaxLength(254)
            .IsRequired(false);
    }
}