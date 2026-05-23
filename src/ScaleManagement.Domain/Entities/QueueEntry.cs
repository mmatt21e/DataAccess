using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// A single position in a <see cref="Queue"/>. Carries the (optional) known
/// participants of the upcoming weighment and links to the resulting
/// <see cref="Transaction"/> once weighed.
/// </summary>
public class QueueEntry : TenantEntityBase
{
    public Guid QueueId { get; set; }
    public Queue Queue { get; set; } = null!;

    /// <summary>Position within the queue; lower is earlier.</summary>
    public int Position { get; set; }

    /// <summary>Higher priority entries may be called ahead of their position.</summary>
    public int Priority { get; set; }

    public QueueEntryStatus Status { get; set; } = QueueEntryStatus.Waiting;

    public Guid? TruckId { get; set; }
    public Truck? Truck { get; set; }

    public Guid? DriverId { get; set; }
    public Driver? Driver { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? MaterialId { get; set; }
    public Material? Material { get; set; }

    public DateTimeOffset EnqueuedAtUtc { get; set; }
    public DateTimeOffset? CalledAtUtc { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }

    /// <summary>The weighment produced when this entry was serviced, if any.</summary>
    public Guid? TransactionId { get; set; }
    public Transaction? Transaction { get; set; }

    public string? Notes { get; set; }
}
