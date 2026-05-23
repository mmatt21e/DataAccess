using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScaleManagement.Domain.Entities;

namespace ScaleManagement.Infrastructure.Persistence.Configurations;

public sealed class TransactionMeasurementConfiguration : IEntityTypeConfiguration<TransactionMeasurement>
{
    public void Configure(EntityTypeBuilder<TransactionMeasurement> builder)
    {
        builder.ToTable("TransactionMeasurements");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MeasurementType).HasConversion<int>();
        builder.Property(m => m.Unit).HasConversion<int>();
        builder.Property(m => m.Value).HasPrecision(18, 4);
        builder.Property(m => m.Source).HasMaxLength(128);
        builder.Property(m => m.Notes).HasMaxLength(1024);
        builder.Property(m => m.CreatedBy).HasMaxLength(256);
        builder.Property(m => m.ModifiedBy).HasMaxLength(256);
        builder.Property(m => m.DeletedBy).HasMaxLength(256);

        builder.HasIndex(m => new { m.TransactionId, m.MeasurementType, m.SequenceNumber });
    }
}
