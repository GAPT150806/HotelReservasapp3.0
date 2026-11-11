using HotelReservasapp.BusinessLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservasapp.DataAccess.Configurations;

public class BedroomTypeConfiguration : IEntityTypeConfiguration<BedroomType>
{
    public void Configure(EntityTypeBuilder<BedroomType> builder)
    {
        builder.ToTable("TiposHabitacion");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Caracteristicas)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasIndex(t => t.Nombre)
            .IsUnique()
            .HasDatabaseName("UQ_TiposHabitacion_Nombre");
    }
}