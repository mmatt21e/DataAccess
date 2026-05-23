using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class TenantModuleConfiguration : IEntityTypeConfiguration<TenantModule>
{
    public void Configure(EntityTypeBuilder<TenantModule> builder)
    {
        builder.ToTable("TenantModules");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.ScaleType).HasConversion<int>();
        builder.Property(m => m.SettingsJson).HasColumnType("nvarchar(max)");
        builder.Property(m => m.CreatedBy).HasMaxLength(256);
        builder.Property(m => m.ModifiedBy).HasMaxLength(256);

        // One module row per scale type per tenant.
        builder.HasIndex(m => new { m.TenantId, m.ScaleType }).IsUnique();
    }
}
