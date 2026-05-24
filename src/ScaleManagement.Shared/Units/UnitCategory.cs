using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Shared.Units;

/// <summary>The physical dimension a <see cref="UnitOfMeasure"/> belongs to.</summary>
public enum UnitCategory
{
    Mass,
    FlowRate,
    Volume,
    Level,
    Temperature
}

/// <summary>Maps each <see cref="UnitOfMeasure"/> to its physical category.</summary>
public static class UnitCategories
{
    public static UnitCategory Of(UnitOfMeasure unit) => unit switch
    {
        UnitOfMeasure.Microgram or UnitOfMeasure.Milligram or UnitOfMeasure.Gram or
        UnitOfMeasure.Kilogram or UnitOfMeasure.Tonne or UnitOfMeasure.Pound or
        UnitOfMeasure.Ounce or UnitOfMeasure.ShortTon or UnitOfMeasure.LongTon
            => UnitCategory.Mass,

        UnitOfMeasure.KilogramPerHour or UnitOfMeasure.TonnePerHour or
        UnitOfMeasure.PoundPerHour or UnitOfMeasure.KilogramPerMinute
            => UnitCategory.FlowRate,

        UnitOfMeasure.Litre or UnitOfMeasure.Millilitre or
        UnitOfMeasure.CubicMetre or UnitOfMeasure.Gallon
            => UnitCategory.Volume,

        UnitOfMeasure.Percent or UnitOfMeasure.Millimetre or
        UnitOfMeasure.Metre or UnitOfMeasure.Count
            => UnitCategory.Level,

        UnitOfMeasure.DegreesCelsius or UnitOfMeasure.DegreesFahrenheit
            => UnitCategory.Temperature,

        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unmapped unit of measure.")
    };
}
