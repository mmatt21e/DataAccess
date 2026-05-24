using ScaleManagement.Domain.Enums;
using ScaleManagement.Shared.Units;

namespace ScaleManagement.Shared.ValueObjects;

/// <summary>
/// A mass value paired with its unit. Entities persist weights as a plain
/// <c>decimal</c> + <see cref="UnitOfMeasure"/> (anemic model); this value object
/// is the convenience type Core services use to compute and compare weights
/// safely. Arithmetic between weights requires identical units — convert
/// explicitly via <see cref="To"/> first to make any unit change visible at the
/// call site.
/// </summary>
public readonly record struct Weight(decimal Value, UnitOfMeasure Unit)
{
    /// <summary>Returns this weight expressed in <paramref name="target"/> using the supplied converter.</summary>
    public Weight To(UnitOfMeasure target, IUnitConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        return Unit == target ? this : new Weight(converter.Convert(Value, Unit, target), target);
    }

    public static Weight operator +(Weight a, Weight b)
    {
        EnsureSameUnit(a, b);
        return new Weight(a.Value + b.Value, a.Unit);
    }

    public static Weight operator -(Weight a, Weight b)
    {
        EnsureSameUnit(a, b);
        return new Weight(a.Value - b.Value, a.Unit);
    }

    public static bool operator <(Weight a, Weight b) { EnsureSameUnit(a, b); return a.Value < b.Value; }
    public static bool operator >(Weight a, Weight b) { EnsureSameUnit(a, b); return a.Value > b.Value; }
    public static bool operator <=(Weight a, Weight b) { EnsureSameUnit(a, b); return a.Value <= b.Value; }
    public static bool operator >=(Weight a, Weight b) { EnsureSameUnit(a, b); return a.Value >= b.Value; }

    public override string ToString() => $"{Value} {Unit}";

    private static void EnsureSameUnit(Weight a, Weight b)
    {
        if (a.Unit != b.Unit)
        {
            throw new InvalidOperationException(
                $"Weights have different units ({a.Unit} vs {b.Unit}); convert with To(...) before combining.");
        }
    }
}
