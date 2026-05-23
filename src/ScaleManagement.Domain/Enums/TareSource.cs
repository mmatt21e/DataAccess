namespace ScaleManagement.Domain.Enums;

/// <summary>Where the tare weight applied to a transaction originated.</summary>
public enum TareSource
{
    None = 0,

    /// <summary>Tare physically weighed during this transaction (two-pass).</summary>
    Measured = 1,

    /// <summary>Tare taken from the truck's stored/permanent tare.</summary>
    Stored = 2,

    /// <summary>Tare entered manually by the operator.</summary>
    Manual = 3
}
