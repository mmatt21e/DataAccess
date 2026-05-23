namespace ScaleManagement.Domain.Abstractions;

/// <summary>
/// Ambient information about the caller for the current unit of work. Supplied
/// by the host (e.g. resolved from the authenticated principal in a web
/// request, or set explicitly in a background/terminal process) and consumed by
/// the persistence layer to apply tenant filtering and stamp provenance/audit
/// fields. Keeping this an abstraction in the domain keeps the persistence layer
/// free of any web/HTTP dependency.
/// </summary>
public interface ITenantContext
{
    /// <summary>The active tenant, or null when running unscoped (migrations, admin tooling).</summary>
    Guid? TenantId { get; }

    /// <summary>Identifier of the current user (for provenance/audit stamping).</summary>
    string? UserId { get; }

    /// <summary>Display name of the current user.</summary>
    string? UserName { get; }

    /// <summary>Correlation/trace id of the current operation, recorded on audit entries.</summary>
    string? CorrelationId { get; }

    /// <summary>True when a tenant is set; when false, tenant query filters are bypassed.</summary>
    bool HasTenant => TenantId.HasValue;
}
