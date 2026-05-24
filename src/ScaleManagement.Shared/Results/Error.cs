namespace ScaleManagement.Shared.Results;

/// <summary>
/// A machine-readable failure descriptor. <see cref="Code"/> is a stable,
/// dot-delimited identifier (e.g. "transaction.tare_exceeds_gross") intended for
/// programmatic handling and localisation; <see cref="Message"/> is a
/// human-readable default.
/// </summary>
public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);
    public static Error Unexpected(string code, string message) => new(code, message, ErrorType.Unexpected);
}
