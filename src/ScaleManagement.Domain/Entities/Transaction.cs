using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// The central record of a single weighing event, produced by any scale type.
/// Frequently-queried weight values (gross/tare/net) are promoted to columns
/// for performance and reporting, while the open-ended set of readings each
/// technology produces — flow rate, batch quantities, inventory level, head
/// count, totalised mass, etc. — is captured in the related
/// <see cref="Measurements"/> collection. This hybrid keeps the schema stable
/// across every module while remaining fully flexible.
/// </summary>
public class Transaction : TenantEntityBase
{
    /// <summary>Human-readable, tenant-unique ticket number.</summary>
    public string TransactionNumber { get; set; } = string.Empty;

    public Guid ScaleId { get; set; }
    public Scale Scale { get; set; } = null!;

    /// <summary>
    /// Denormalised scale technology, copied from the originating scale. Lets the
    /// large transaction table be filtered/partitioned per module cheaply.
    /// </summary>
    public ScaleType ScaleType { get; set; }

    public Guid? MaterialId { get; set; }
    public Material? Material { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? DriverId { get; set; }
    public Driver? Driver { get; set; }

    public Guid? TruckId { get; set; }
    public Truck? Truck { get; set; }

    public Guid? OperatorUserId { get; set; }
    public User? OperatorUser { get; set; }

    public TransactionDirection Direction { get; set; } = TransactionDirection.Unspecified;
    public TransactionStatus Status { get; set; } = TransactionStatus.Open;
    public WeighingMode WeighingMode { get; set; } = WeighingMode.SingleDraft;

    // --- Promoted weight values (nullable: not every scale type populates them) ---
    public decimal? GrossWeight { get; set; }
    public decimal? TareWeight { get; set; }
    public decimal? NetWeight { get; set; }
    public UnitOfMeasure WeightUnit { get; set; } = UnitOfMeasure.Kilogram;
    public TareSource TareSource { get; set; } = TareSource.None;

    // --- Timing for multi-pass weighing ---
    public DateTimeOffset? FirstWeighedAtUtc { get; set; }
    public DateTimeOffset? SecondWeighedAtUtc { get; set; }
    public DateTimeOffset? StartedAtUtc { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }

    // --- Ticketing / references ---
    public bool TicketPrinted { get; set; }
    public string? TicketNumber { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }

    /// <summary>All readings captured for this weighment, of any measurement type.</summary>
    public ICollection<TransactionMeasurement> Measurements { get; set; } = new List<TransactionMeasurement>();
}
