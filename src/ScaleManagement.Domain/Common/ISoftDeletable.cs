namespace ScaleManagement.Domain.Common;

/// <summary>
/// Entities implementing this contract are never physically removed; deletes are
/// converted to a flag update by the persistence layer and hidden from queries
/// by a global query filter. Weighing records frequently have regulatory and
/// billing retention requirements, so soft deletion is the default.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTimeOffset? DeletedAtUtc { get; set; }
    string? DeletedBy { get; set; }
}
