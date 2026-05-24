namespace ScaleManagement.Shared.Results;

/// <summary>
/// Classifies a failure so callers (and the eventual API layer) can map it to
/// the right response without string-matching messages.
/// </summary>
public enum ErrorType
{
    /// <summary>Input or domain-rule violation (e.g. tare exceeds gross).</summary>
    Validation = 0,

    /// <summary>A referenced entity does not exist (or is not visible to the tenant).</summary>
    NotFound = 1,

    /// <summary>The operation conflicts with current state (e.g. already completed).</summary>
    Conflict = 2,

    /// <summary>The caller is not permitted to perform the operation.</summary>
    Forbidden = 3,

    /// <summary>An unexpected condition; typically surfaced as a 500-class error.</summary>
    Unexpected = 4
}
