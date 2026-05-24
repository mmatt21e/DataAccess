using ScaleManagement.Domain.Abstractions;

namespace ScaleManagement.Shared.Time;

/// <summary>
/// A tenant-timezone-aware view over the system <see cref="IClock"/>. Reporting
/// and ticketing need "the current tenant's local day" (e.g. for daily-throughput
/// rollups and printed timestamps), which the raw UTC <see cref="IClock"/> cannot
/// provide. The implementation resolves the active tenant's configured time zone;
/// it lives above the DAL because that resolution needs tenant data.
/// </summary>
public interface ITenantClock
{
    /// <summary>Current instant in UTC (delegates to <see cref="IClock"/>).</summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>Current instant expressed in the active tenant's local time zone.</summary>
    DateTimeOffset LocalNow { get; }

    /// <summary>The active tenant's local calendar date.</summary>
    DateOnly Today { get; }

    /// <summary>Converts a UTC instant to the active tenant's local time.</summary>
    DateTimeOffset ToTenantLocal(DateTimeOffset utc);
}
