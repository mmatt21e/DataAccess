using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class AuditConfigurationConfiguration : IEntityTypeConfiguration<AuditConfiguration>
{
    public void Configure(EntityTypeBuilder<AuditConfiguration> builder)
    {
        builder.ToTable("AuditConfigurations");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.EntityName).HasMaxLength(256).IsRequired();
        builder.Property(c => c.ExcludedProperties).HasMaxLength(2048);
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.ModifiedBy).HasMaxLength(256);

        // One config per (tenant, entity); the global default has TenantId == null.
        // SQL Server treats NULLs as equal in a unique index, so only one global
        // row per entity name is allowed — exactly the intended behaviour.
        builder.HasIndex(c => new { c.TenantId, c.EntityName }).IsUnique();
    }
}
