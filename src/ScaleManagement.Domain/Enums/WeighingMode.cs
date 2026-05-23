namespace ScaleManagement.Domain.Enums;

/// <summary>
/// How a weighment is acquired. Different scale technologies naturally use
/// different modes; storing it on the transaction lets reporting and the UI
/// adapt without inspecting the scale type.
/// </summary>
public enum WeighingMode
{
    /// <summary>Single static draft (e.g. crane scale, simple inbound weight).</summary>
    SingleDraft = 1,

    /// <summary>Two-pass weighing: first weight then second weight yields net (truck scales).</summary>
    TwoPass = 2,

    /// <summary>Stored/permanent tare applied to a single live weight (truck scales).</summary>
    StoredTare = 3,

    /// <summary>Repeated gravimetric drafts accumulating to a target (hopper batching).</summary>
    Batch = 4,

    /// <summary>Continuous in-motion accumulation (conveyor belt scales).</summary>
    Continuous = 5,

    /// <summary>Motion-tolerant dynamic capture (livestock / checkweigh).</summary>
    Dynamic = 6
}
