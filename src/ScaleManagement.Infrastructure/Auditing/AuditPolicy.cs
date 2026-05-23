using ScaleManagement.Domain.Common;

namespace ScaleManagement.Infrastructure.Auditing;

/// <summary>
/// The resolved, ready-to-apply auditing decision for one entity type, derived
/// from an <c>AuditConfiguration</c> row. Immutable so it can be cached and
/// shared safely across threads.
/// </summary>
public sealed class AuditPolicy
{
    public AuditPolicy(
        bool onInsert,
        bool onUpdate,
        bool onDelete,
        bool captureOldValues,
        bool captureNewValues,
        IReadOnlySet<string> excludedProperties)
    {
        OnInsert = onInsert;
        OnUpdate = onUpdate;
        OnDelete = onDelete;
        CaptureOldValues = captureOldValues;
        CaptureNewValues = captureNewValues;
        ExcludedProperties = excludedProperties;
    }

    public bool OnInsert { get; }
    public bool OnUpdate { get; }
    public bool OnDelete { get; }
    public bool CaptureOldValues { get; }
    public bool CaptureNewValues { get; }
    public IReadOnlySet<string> ExcludedProperties { get; }

    /// <summary>True when the given action should produce an audit entry under this policy.</summary>
    public bool ShouldAudit(AuditAction action) => action switch
    {
        AuditAction.Insert => OnInsert,
        AuditAction.Update => OnUpdate,
        AuditAction.Delete => OnDelete,
        _ => false
    };
}
