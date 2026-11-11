using HotelReservasapp.BusinessLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservasapp.DataAccess.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hoteles", table =>
        {
            table.HasCheckConstraint("CK_Hoteles_Estado", "[Estado] IN ('Activo', 'Inactivo')");
        });

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(h => h.Nombre)
            .IsUnique()
            .HasDatabaseName("UQ_Hoteles_Nombre"); ;

        builder.Property(h => h.Estado)
            .HasDefaultValueSql("'Activo'")
            .HasColumnType("varchar(15)")
            .HasConversion<string>()
            .IsRequired();
    }
}