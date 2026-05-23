using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Drivers");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName).HasMaxLength(128).IsRequired();
        builder.Property(d => d.LastName).HasMaxLength(128).IsRequired();
        builder.Property(d => d.LicenseNumber).HasMaxLength(64);
        builder.Property(d => d.Phone).HasMaxLength(64);
        builder.Property(d => d.CreatedBy).HasMaxLength(256);
        builder.Property(d => d.ModifiedBy).HasMaxLength(256);
        builder.Property(d => d.DeletedBy).HasMaxLength(256);

        builder.HasIndex(d => new { d.TenantId, d.LicenseNumber });
        builder.HasIndex(d => d.CompanyId);
    }
}
