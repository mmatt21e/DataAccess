namespace ScaleManagement.Domain.Enums;

/// <summary>
/// The kind of value a single captured reading represents. Common weight values
/// (gross/tare/net) are also promoted to first-class columns on
/// <c>Transaction</c> for fast querying, but every reading — including
/// technology-specific ones like flow rate, inventory level, batch quantity,
/// head count and totalised mass — is stored uniformly as a
/// <c>TransactionMeasurement</c>. This keeps the schema stable while supporting
/// arbitrary measurement shapes across all scale modules.
/// </summary>
public enum MeasurementType
{
    GrossWeight = 1,
    TareWeight = 2,
    NetWeight = 3,

    /// <summary>Quantity weighed/dispensed in a batching draft (hopper scales).</summary>
    BatchQuantity = 10,

    /// <summary>Instantaneous throughput (belt scales), value paired with a rate unit.</summary>
    FlowRate = 11,

    /// <summary>Accumulated mass over a run (belt totaliser).</summary>
    TotalisedWeight = 12,

    /// <summary>Stored contents of a tank/silo expressed as level or mass.</summary>
    InventoryLevel = 13,

    /// <summary>Number of items/animals in a dynamic or counting weighment.</summary>
    Count = 14,

    /// <summary>Mean per-unit weight (livestock / checkweigh statistics).</summary>
    AverageWeight = 15,

    /// <summary>Volume derived from mass and density (tank/silo).</summary>
    Volume = 16,

    /// <summary>Measured deviation from a target (checkweigher).</summary>
    Deviation = 17,

    /// <summary>Temperature captured alongside a weighment (compensation / quality).</summary>
    Temperature = 18,

    /// <summary>Moisture / humidity reading captured with the weighment.</summary>
    Moisture = 19
}
