using ScaleManagement.Domain.Common;
using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Entities;

/// <summary>
/// A physical weighing device. A single tenant may operate many scales of
/// different <see cref="ScaleType"/>s; the <see cref="ScaleType"/> ties the
/// device back to the tenant's enabled <c>TenantModule</c>.
/// </summary>
public class Scale : TenantEntityBase
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Tenant-unique code/identifier for the scale (e.g. "WB-01").</summary>
    public string Code { get; set; } = string.Empty;

    public ScaleType ScaleType { get; set; }

    /// <summary>Default weighing mode the scale operates in.</summary>
    public WeighingMode WeighingMode { get; set; } = WeighingMode.SingleDraft;

    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? Location { get; set; }

    /// <summary>Network/device address the terminal driver connects to (host:port, COM port, etc.).</summary>
    public string? DeviceAddress { get; set; }

    /// <summary>Maximum rated capacity, expressed in <see cref="CapacityUnit"/>.</summary>
    public decimal? Capacity { get; set; }

    /// <summary>Smallest displayable increment (verification scale interval / "e").</summary>
    public decimal? Division { get; set; }

    public UnitOfMeasure CapacityUnit { get; set; } = UnitOfMeasure.Kilogram;

    /// <summary>Default unit applied to weighments captured on this scale.</summary>
    public UnitOfMeasure DefaultUnit { get; set; } = UnitOfMeasure.Kilogram;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset? LastCalibratedAtUtc { get; set; }
    public DateTimeOffset? NextCalibrationDueUtc { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Queue> Queues { get; set; } = new List<Queue>();
}
