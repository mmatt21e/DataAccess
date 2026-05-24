using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Shared.Units;

/// <summary>
/// Converts a measured value between units of the same physical category.
/// Implementations are pure and stateless; cross-category conversions (e.g.
/// mass to volume) are intentionally unsupported and throw.
/// </summary>
public interface IUnitConverter
{
    /// <summary>True when <paramref name="from"/> and <paramref name="to"/> can be inter-converted.</summary>
    bool CanConvert(UnitOfMeasure from, UnitOfMeasure to);

    /// <summary>
    /// Converts <paramref name="value"/> from one unit to another.
    /// Throws <see cref="InvalidOperationException"/> when the units are not
    /// convertible (different categories or an unsupported category).
    /// </summary>
    decimal Convert(decimal value, UnitOfMeasure from, UnitOfMeasure to);
}
