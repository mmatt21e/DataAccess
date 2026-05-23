using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class ScaleConfiguration : IEntityTypeConfiguration<Scale>
{
    public void Configure(EntityTypeBuilder<Scale> builder)
    {
        builder.ToTable("Scales");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(256).IsRequired();
        builder.Property(s => s.Code).HasMaxLength(64).IsRequired();
        builder.Property(s => s.Manufacturer).HasMaxLength(128);
        builder.Property(s => s.Model).HasMaxLength(128);
        builder.Property(s => s.SerialNumber).HasMaxLength(128);
        builder.Property(s => s.Location).HasMaxLength(256);
        builder.Property(s => s.DeviceAddress).HasMaxLength(256);
        builder.Property(s => s.CreatedBy).HasMaxLength(256);
        builder.Property(s => s.ModifiedBy).HasMaxLength(256);
        builder.Property(s => s.DeletedBy).HasMaxLength(256);

        builder.Property(s => s.ScaleType).HasConversion<int>();
        builder.Property(s => s.WeighingMode).HasConversion<int>();
        builder.Property(s => s.CapacityUnit).HasConversion<int>();
        builder.Property(s => s.DefaultUnit).HasConversion<int>();

        builder.Property(s => s.Capacity).HasPrecision(18, 3);
        builder.Property(s => s.Division).HasPrecision(18, 3);

        builder.HasIndex(s => new { s.TenantId, s.Code }).IsUnique();
        builder.HasIndex(s => new { s.TenantId, s.ScaleType });
        builder.HasIndex(s => new { s.TenantId, s.IsActive });
    }
}
