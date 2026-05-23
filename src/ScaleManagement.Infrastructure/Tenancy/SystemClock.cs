using ScaleManagement.Domain.Abstractions;

namespace ScaleManagement.Infrastructure.Tenancy;

/// <summary>Default <see cref="IClock"/> backed by the machine clock.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
