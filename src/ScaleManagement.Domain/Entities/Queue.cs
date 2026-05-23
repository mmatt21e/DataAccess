using ScaleManagement.Domain.Common;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// An ordered waiting line of vehicles/loads for a scale or weighing process.
/// A tenant can run several queues (e.g. inbound vs outbound, or one per scale).
/// </summary>
public class Queue : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Optional scale this queue feeds. Null for a process-level queue.</summary>
    public Guid? ScaleId { get; set; }
    public Scale? Scale { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<QueueEntry> Entries { get; set; } = new List<QueueEntry>();
}
