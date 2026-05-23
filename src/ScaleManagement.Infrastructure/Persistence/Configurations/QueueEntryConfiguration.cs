using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
{
    public void Configure(EntityTypeBuilder<QueueEntry> builder)
    {
        builder.ToTable("QueueEntries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status).HasConversion<int>();
        builder.Property(e => e.Notes).HasMaxLength(1024);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.ModifiedBy).HasMaxLength(256);
        builder.Property(e => e.DeletedBy).HasMaxLength(256);

        builder.HasIndex(e => new { e.QueueId, e.Status, e.Priority, e.Position });

        builder.HasOne(e => e.Truck)
            .WithMany()
            .HasForeignKey(e => e.TruckId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Driver)
            .WithMany()
            .HasForeignKey(e => e.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Company)
            .WithMany()
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Material)
            .WithMany()
            .HasForeignKey(e => e.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Transaction)
            .WithMany()
            .HasForeignKey(e => e.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
