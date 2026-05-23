using ScaleManagement.Domain.Abstractions;

namespace ScaleManagement.Infrastructure.Tenancy;

/// <summary>
/// A simple mutable <see cref="ITenantContext"/> suitable for registration as a
/// scoped service. In a web host a small piece of middleware sets these values
/// from the authenticated principal at the start of each request; in background
/// or terminal processes the host sets them explicitly before opening a unit of
/// work. Implemented as a settable POCO so it stays host-agnostic.
/// </summary>
public sealed class AmbientTenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public string? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string? CorrelationId { get; private set; }

    /// <summary>Establishes the tenant/user for the current scope.</summary>
    public void Set(Guid? tenantId, string? userId = null, string? userName = null, string? correlationId = null)
    {
        TenantId = tenantId;
        UserId = userId;
        UserName = userName;
        CorrelationId = correlationId;
    }

    /// <summary>Clears the context back to an unscoped state.</summary>
    public void Clear() => Set(null);
}
