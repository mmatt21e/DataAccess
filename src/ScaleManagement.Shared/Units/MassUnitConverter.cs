using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Shared.Units;

/// <summary>
/// Converts between mass units using exact gram-based factors. Conversions
/// involving any non-mass unit are rejected. Additional converters (flow rate,
/// volume) can be introduced alongside this one as modules require them.
/// </summary>
public sealed class MassUnitConverter : IUnitConverter
{
    // Factor = number of grams in one of the unit.
    private static readonly IReadOnlyDictionary<UnitOfMeasure, decimal> GramsPerUnit =
        new Dictionary<UnitOfMeasure, decimal>
        {
            [UnitOfMeasure.Microgram] = 0.000001m,
            [UnitOfMeasure.Milligram] = 0.001m,
            [UnitOfMeasure.Gram] = 1m,
            [UnitOfMeasure.Kilogram] = 1000m,
            [UnitOfMeasure.Tonne] = 1_000_000m,
            [UnitOfMeasure.Pound] = 453.59237m,
            [UnitOfMeasure.Ounce] = 28.349523125m,
            [UnitOfMeasure.ShortTon] = 907_184.74m,
            [UnitOfMeasure.LongTon] = 1_016_046.9088m
        };

    public bool CanConvert(UnitOfMeasure from, UnitOfMeasure to)
        => GramsPerUnit.ContainsKey(from) && GramsPerUnit.ContainsKey(to);

    public decimal Convert(decimal value, UnitOfMeasure from, UnitOfMeasure to)
    {
        if (from == to)
            return value;

        if (!GramsPerUnit.TryGetValue(from, out var fromGrams) ||
            !GramsPerUnit.TryGetValue(to, out var toGrams))
        {
            throw new InvalidOperationException(
                $"Cannot convert from '{from}' to '{to}': both must be mass units.");
        }

        return value * fromGrams / toGrams;
    }
}
