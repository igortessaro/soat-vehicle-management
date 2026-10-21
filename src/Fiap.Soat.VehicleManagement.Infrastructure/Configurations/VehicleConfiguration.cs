using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.Soat.VehicleManagement.Infrastructure.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime(6)")
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime(6)")
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("vehicle_type")
            .HasConversion<string>()
            .HasColumnType("VARCHAR(30)")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Brand)
            .HasColumnName("brand")
            .HasColumnType("VARCHAR(100)")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Model)
            .HasColumnName("model")
            .HasColumnType("VARCHAR(150)")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.LicensePlate)
            .HasColumnName("license_plate")
            .HasColumnType("VARCHAR(20)")
            .HasMaxLength(20)
            .HasConversion(
                licensePlate => licensePlate.Value,
                value => (LicensePlate) value)
            .IsRequired();

        builder.HasIndex(x => x.LicensePlate).IsUnique();

        builder.Property(x => x.Color)
            .HasColumnName("color")
            .HasColumnType("VARCHAR(50)")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasColumnType("VARCHAR(30)")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ManufactureYear)
            .HasColumnName("manufacture_year")
            .HasColumnType("INT")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .HasColumnType("DECIMAL(12,2)")
            .IsRequired();

        builder.HasIndex(x => new { x.Status, x.Price }).HasDatabaseName("idx_vehicle_status_price");
    }
}
