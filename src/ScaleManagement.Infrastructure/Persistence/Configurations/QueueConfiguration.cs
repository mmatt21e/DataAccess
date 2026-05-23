using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class QueueConfiguration : IEntityTypeConfiguration<Queue>
{
    public void Configure(EntityTypeBuilder<Queue> builder)
    {
        builder.ToTable("Queues");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Name).HasMaxLength(256).IsRequired();
        builder.Property(q => q.Description).HasMaxLength(1024);
        builder.Property(q => q.CreatedBy).HasMaxLength(256);
        builder.Property(q => q.ModifiedBy).HasMaxLength(256);
        builder.Property(q => q.DeletedBy).HasMaxLength(256);

        builder.HasIndex(q => new { q.TenantId, q.IsActive });

        builder.HasOne(q => q.Scale)
            .WithMany(s => s.Queues)
            .HasForeignKey(q => q.ScaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(q => q.Entries)
            .WithOne(e => e.Queue)
            .HasForeignKey(e => e.QueueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
