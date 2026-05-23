using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// Associates a tenant with a single licensed/enabled scale module. This is the
/// mechanism that lets different customers use different scale technologies
/// independently <b>without code changes</b>: enabling truck scales for one
/// tenant and conveyor belt scales for another is a matter of inserting rows
/// here. Module-specific behaviour is configured through <see cref="SettingsJson"/>
/// rather than compiled-in branches.
/// </summary>
public class TenantModule : BaseEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    /// <summary>The scale technology/module this row enables.</summary>
    public ScaleType ScaleType { get; set; }

    public bool IsEnabled { get; set; } = true;

    /// <summary>Optional cap on the number of scales of this type the tenant may register.</summary>
    public int? LicensedScaleLimit { get; set; }

    /// <summary>Free-form module configuration (thresholds, ticket templates, integration ids…) stored as JSON.</summary>
    public string? SettingsJson { get; set; }

    public DateTimeOffset? ValidFromUtc { get; set; }
    public DateTimeOffset? ValidUntilUtc { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAtUtc { get; set; }
    public string? ModifiedBy { get; set; }
}
