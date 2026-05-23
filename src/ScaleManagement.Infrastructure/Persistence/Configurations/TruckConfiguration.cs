using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class TruckConfiguration : IEntityTypeConfiguration<Truck>
{
    public void Configure(EntityTypeBuilder<Truck> builder)
    {
        builder.ToTable("Trucks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.LicensePlate).HasMaxLength(32).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(256);
        builder.Property(t => t.Make).HasMaxLength(128);
        builder.Property(t => t.CreatedBy).HasMaxLength(256);
        builder.Property(t => t.ModifiedBy).HasMaxLength(256);
        builder.Property(t => t.DeletedBy).HasMaxLength(256);

        builder.Property(t => t.StoredTareUnit).HasConversion<int>();
        builder.Property(t => t.StoredTareWeight).HasPrecision(18, 3);

        builder.HasIndex(t => new { t.TenantId, t.LicensePlate }).IsUnique();
        builder.HasIndex(t => t.CompanyId);
    }
}
