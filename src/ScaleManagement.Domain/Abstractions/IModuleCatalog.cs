using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Abstractions;

/// <summary>
/// Read model over the active tenant's enabled scale modules (the
/// <c>TenantModule</c> rows). This is the gate the business layer consults
/// before performing module-specific work, so enabling a scale technology for a
/// tenant stays a data operation rather than a code change. Lookups are expected
/// to be cached; results already account for the module's validity window.
/// <para>
/// The raw <see cref="ModuleDescriptor.SettingsJson"/> is returned as-is; typed
/// binding to a module's settings type is the responsibility of the module SDK,
/// keeping the domain free of module-specific types.
/// </para>
/// </summary>
public interface IModuleCatalog
{
    /// <summary>True when the module for <paramref name="scaleType"/> is enabled and currently valid for the active tenant.</summary>
    bool IsEnabled(ScaleType scaleType);

    /// <summary>The scale types currently enabled for the active tenant.</summary>
    IReadOnlyCollection<ScaleType> EnabledModules();

    /// <summary>The descriptor for a module, or null when the tenant has no row for it.</summary>
    ModuleDescriptor? Get(ScaleType scaleType);
}

/// <summary>An immutable snapshot of a tenant's configuration for one scale module.</summary>
public sealed record ModuleDescriptor(
    ScaleType ScaleType,
    bool IsEnabled,
    int? LicensedScaleLimit,
    string? SettingsJson,
    DateTimeOffset? ValidFromUtc,
    DateTimeOffset? ValidUntilUtc);
