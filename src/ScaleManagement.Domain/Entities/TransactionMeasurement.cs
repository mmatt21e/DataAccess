using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// A single typed reading belonging to a <see cref="Transaction"/>. This is the
/// extensibility point that lets one transaction shape serve every scale
/// technology: a belt scale emits FlowRate + TotalisedWeight rows, a silo scale
/// an InventoryLevel row, a hopper scale a sequence of BatchQuantity rows, a
/// livestock scale Count + AverageWeight rows, and so on — all without schema
/// changes. <see cref="SequenceNumber"/> preserves ordering for repeated
/// readings (batch drafts, time-series totaliser snapshots).
/// </summary>
public class TransactionMeasurement : TenantEntityBase
{
    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;

    public MeasurementType MeasurementType { get; set; }

    public decimal Value { get; set; }

    public UnitOfMeasure Unit { get; set; } = UnitOfMeasure.Kilogram;

    /// <summary>Ordinal for repeated readings within the same transaction (1-based).</summary>
    public int SequenceNumber { get; set; }

    public DateTimeOffset CapturedAtUtc { get; set; }

    /// <summary>Originating device/channel tag (useful for multi-sensor scales).</summary>
    public string? Source { get; set; }

    public string? Notes { get; set; }
}
