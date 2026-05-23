namespace ScaleManagement.Domain.Enums;

/// <summary>
/// Engineering unit attached to a measurement. Storing the unit alongside every
/// value (rather than assuming a single system-wide unit) lets one tenant run
/// metric truck scales while another runs imperial livestock scales without any
/// code change.
/// </summary>
public enum UnitOfMeasure
{
    // Mass
    Microgram = 1,
    Milligram = 2,
    Gram = 3,
    Kilogram = 4,
    Tonne = 5,
    Pound = 6,
    Ounce = 7,
    ShortTon = 8,
    LongTon = 9,

    // Flow rate (mass / time)
    KilogramPerHour = 30,
    TonnePerHour = 31,
    PoundPerHour = 32,
    KilogramPerMinute = 33,

    // Volume
    Litre = 50,
    Millilitre = 51,
    CubicMetre = 52,
    Gallon = 53,

    // Level / dimensionless
    Percent = 70,
    Millimetre = 71,
    Metre = 72,
    Count = 73,

    // Environmental
    DegreesCelsius = 90,
    DegreesFahrenheit = 91
}
