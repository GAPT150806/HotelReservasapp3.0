using HotelReservasapp.BusinessLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelReservasapp.DataAccess.Configurations;

public class FloorConfiguration : IEntityTypeConfiguration<Floor>
{
    public void Configure(EntityTypeBuilder<Floor> builder)
    {
        builder.ToTable("Pisos", table =>
        {
            table.HasCheckConstraint("CK_Pisos_Numero", "[Numero] > 0");

            table.HasCheckConstraint("CK_Pisos_Estado", "[Estado] IN ('Activo', 'Inactivo')");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Numero)
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasDefaultValueSql("'Activo'")
            .HasColumnType("varchar(15)")
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(p => p.IdHotel)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasAlternateKey(p => new { p.Id, p.IdHotel })
            .HasName("UQ_Pisos_Id_IdHotel");

        builder.HasIndex(p => new { p.IdHotel, p.Numero })
            .IsUnique()
            .HasDatabaseName("UX_Pisos_Hotel_Numero_Activo")
            .HasFilter("[Estado] = 'Activo'");
    }
}