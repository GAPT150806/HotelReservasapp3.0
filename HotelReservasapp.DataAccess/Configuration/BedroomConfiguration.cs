using HotelReservasapp.BusinessLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservasapp.DataAccess.Configurations;

public class BedroomConfiguration : IEntityTypeConfiguration<Bedroom>
{
    public void Configure(EntityTypeBuilder<Bedroom> builder)
    {
        builder.ToTable("Habitaciones", table =>
        {
            table.HasCheckConstraint("CK_Habitaciones_Numero", "[Numero] > 0");

            table.HasCheckConstraint("CK_Habitaciones_Capacidad", "[Capacidad] > 0");

            table.HasCheckConstraint("CK_Habitaciones_Tarifa", "[TarifaPorNoche] >= 0");

            table.HasCheckConstraint("CK_Habitaciones_Estado", "[Estado] IN ('Disponible', 'Reservada', 'Ocupada', 'Limpieza', 'Mantenimiento', 'FueraServicio')");
        });

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Numero)
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(h => h.Capacidad)
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(h => h.TarifaPorNoche)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(h => h.Estado)
            .HasDefaultValueSql("'Disponible'")
            .HasColumnType("varchar(20)")
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne<Floor>()
            .WithMany()
            .HasForeignKey(h => new { h.IdPiso, h.IdHotel })
            .HasPrincipalKey(p => new { p.Id, p.IdHotel })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<BedroomType>()
            .WithMany()
            .HasForeignKey(h => h.IdTipoHabitacion)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => new { h.IdHotel, h.Numero })
             .IsUnique()
             .HasDatabaseName("UQ_Habitaciones_Hotel_Numero");
    }
}