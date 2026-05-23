namespace ScaleManagement.Domain.Enums;

/// <summary>Lifecycle state of a vehicle/load waiting in a weighing queue.</summary>
public enum QueueEntryStatus
{
    Waiting = 1,
    Called = 2,
    InProgress = 3,
    Completed = 4,
    Skipped = 5,
    NoShow = 6,
    Cancelled = 7
}
