namespace ScaleManagement.Domain.Common;

/// <summary>
/// Marker contract for every persistable aggregate/entity. A single surrogate
/// key type (<see cref="Guid"/>) is used across the model so that the generic
/// repository can operate uniformly and keys can be generated client-side
/// (important for offline scale terminals and for capturing audit records
/// before <c>SaveChanges</c> completes).
/// </summary>
public interface IEntity
{
    Guid Id { get; }
}
