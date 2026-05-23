using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TransactionNumber).HasMaxLength(64).IsRequired();
        builder.Property(t => t.TicketNumber).HasMaxLength(64);
        builder.Property(t => t.ReferenceNumber).HasMaxLength(128);
        builder.Property(t => t.Notes).HasMaxLength(2048);
        builder.Property(t => t.CreatedBy).HasMaxLength(256);
        builder.Property(t => t.ModifiedBy).HasMaxLength(256);
        builder.Property(t => t.DeletedBy).HasMaxLength(256);

        builder.Property(t => t.ScaleType).HasConversion<int>();
        builder.Property(t => t.Direction).HasConversion<int>();
        builder.Property(t => t.Status).HasConversion<int>();
        builder.Property(t => t.WeighingMode).HasConversion<int>();
        builder.Property(t => t.TareSource).HasConversion<int>();
        builder.Property(t => t.WeightUnit).HasConversion<int>();

        builder.Property(t => t.GrossWeight).HasPrecision(18, 3);
        builder.Property(t => t.TareWeight).HasPrecision(18, 3);
        builder.Property(t => t.NetWeight).HasPrecision(18, 3);

        builder.HasIndex(t => new { t.TenantId, t.TransactionNumber }).IsUnique();
        builder.HasIndex(t => new { t.TenantId, t.Status });
        builder.HasIndex(t => new { t.TenantId, t.ScaleType });
        builder.HasIndex(t => new { t.TenantId, t.CreatedAtUtc });
        builder.HasIndex(t => t.ScaleId);
        builder.HasIndex(t => t.CompanyId);
        builder.HasIndex(t => t.TruckId);

        // All master-data relationships are Restrict: weighing history must never be
        // cascade-deleted when a scale/company/etc. is removed. Soft delete keeps the
        // referenced rows around anyway.
        builder.HasOne(t => t.Scale)
            .WithMany(s => s.Transactions)
            .HasForeignKey(t => t.ScaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Material)
            .WithMany(m => m.Transactions)
            .HasForeignKey(t => t.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Company)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Driver)
            .WithMany(d => d.Transactions)
            .HasForeignKey(t => t.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Truck)
            .WithMany(tr => tr.Transactions)
            .HasForeignKey(t => t.TruckId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.OperatorUser)
            .WithMany(u => u.OperatedTransactions)
            .HasForeignKey(t => t.OperatorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Measurements)
            .WithOne(m => m.Transaction)
            .HasForeignKey(m => m.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
