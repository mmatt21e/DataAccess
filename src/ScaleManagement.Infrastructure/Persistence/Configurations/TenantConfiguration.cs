using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Code).HasMaxLength(64).IsRequired();
        builder.Property(t => t.TimeZoneId).HasMaxLength(64).IsRequired();
        builder.Property(t => t.DefaultCulture).HasMaxLength(16).IsRequired();
        builder.Property(t => t.CreatedBy).HasMaxLength(256);
        builder.Property(t => t.ModifiedBy).HasMaxLength(256);

        builder.HasIndex(t => t.Code).IsUnique();

        builder.HasMany(t => t.Modules)
            .WithOne(m => m.Tenant)
            .HasForeignKey(m => m.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
