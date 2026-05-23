using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityName).HasMaxLength(256).IsRequired();
        builder.Property(a => a.EntityId).HasMaxLength(128).IsRequired();
        builder.Property(a => a.Action).HasConversion<int>();
        builder.Property(a => a.OldValues).HasColumnType("nvarchar(max)");
        builder.Property(a => a.NewValues).HasColumnType("nvarchar(max)");
        builder.Property(a => a.ChangedColumns).HasColumnType("nvarchar(max)");
        builder.Property(a => a.UserId).HasMaxLength(256);
        builder.Property(a => a.UserName).HasMaxLength(256);
        builder.Property(a => a.CorrelationId).HasMaxLength(128);

        builder.HasIndex(a => new { a.EntityName, a.EntityId });
        builder.HasIndex(a => new { a.TenantId, a.TimestampUtc });
        builder.HasIndex(a => a.TimestampUtc);

        // Append-only: no navigation properties / FKs are defined intentionally.
    }
}
