namespace ScaleManagement.Domain.Abstractions;

/// <summary>
/// Abstraction over the system clock so that time-dependent persistence
/// behaviour (provenance stamps, audit timestamps) is deterministically
/// testable.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
