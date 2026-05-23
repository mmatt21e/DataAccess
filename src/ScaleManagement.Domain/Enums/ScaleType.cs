namespace ScaleManagement.Domain.Enums;

/// <summary>
/// The weighing technology a scale represents. Each value corresponds to a
/// "scale module" that a tenant can independently license/enable via
/// <c>TenantModule</c> — adding a new technology is a matter of adding a value
/// here and a data row, not changing branching logic across the application.
/// </summary>
public enum ScaleType
{
    /// <summary>Vehicle / weighbridge scale (static gross-tare-net weighing).</summary>
    TruckScale = 1,

    /// <summary>Gravimetric hopper scale used for batching by weight.</summary>
    HopperScale = 2,

    /// <summary>In-motion conveyor belt scale producing flow rate and totalised mass.</summary>
    ConveyorBeltScale = 3,

    /// <summary>Tank scale measuring contained mass / level.</summary>
    TankScale = 4,

    /// <summary>Silo scale (load-cell mounted) measuring stored inventory level.</summary>
    SiloScale = 5,

    /// <summary>Crane / hook scale weighing suspended loads.</summary>
    CraneScale = 6,

    /// <summary>Checkweigher performing in-motion pass/fail weight verification.</summary>
    CheckweighScale = 7,

    /// <summary>Livestock scale performing dynamic (motion-tolerant) animal weighing.</summary>
    LivestockScale = 8
}
