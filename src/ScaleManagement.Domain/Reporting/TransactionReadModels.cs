using ScaleManagement.Domain.Enums;

namespace ScaleManagement.Domain.Reporting;

/// <summary>
/// Flat, query-shaped projection of a transaction for list/report views. These
/// records are intentionally separate from the <c>Transaction</c> entity: the
/// reporting side reads denormalised, display-ready rows and never tracks or
/// mutates entities.
/// </summary>
public sealed record TransactionSummaryRow(
    Guid Id,
    string TransactionNumber,
    DateTimeOffset? CompletedAtUtc,
    Guid ScaleId,
    string ScaleName,
    ScaleType ScaleType,
    string? MaterialName,
    string? CompanyName,
    decimal? NetWeight,
    UnitOfMeasure WeightUnit,
    TransactionStatus Status,
    TransactionDirection Direction);

/// <summary>Net-weight totals and transaction counts grouped by material over a date range.</summary>
public sealed record MaterialTotalsRow(
    Guid? MaterialId,
    string? MaterialName,
    int TransactionCount,
    decimal TotalNetWeight,
    UnitOfMeasure WeightUnit);

/// <summary>Per-day throughput for a scale: completed transactions and summed net weight.</summary>
public sealed record DailyThroughputRow(
    DateOnly Date,
    int TransactionCount,
    decimal TotalNetWeight,
    UnitOfMeasure WeightUnit);
