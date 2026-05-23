using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).HasMaxLength(256).IsRequired();
        builder.Property(m => m.Code).HasMaxLength(64).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(1024);
        builder.Property(m => m.Category).HasMaxLength(128);
        builder.Property(m => m.CreatedBy).HasMaxLength(256);
        builder.Property(m => m.ModifiedBy).HasMaxLength(256);
        builder.Property(m => m.DeletedBy).HasMaxLength(256);

        builder.Property(m => m.DefaultUnit).HasConversion<int>();
        builder.Property(m => m.Density).HasPrecision(18, 6);
        builder.Property(m => m.UnitPrice).HasPrecision(18, 4);

        builder.HasIndex(m => new { m.TenantId, m.Code }).IsUnique();
    }
}
