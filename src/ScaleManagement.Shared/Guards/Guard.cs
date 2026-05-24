using System.Runtime.CompilerServices;

namespace ScaleManagement.Shared.Guards;

/// <summary>
/// Lightweight argument guards for the system boundary. Internal code generally
/// trusts its inputs; use these where external/untrusted values enter a service.
/// </summary>
public static class Guard
{
    public static T AgainstNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        where T : class
        => value ?? throw new ArgumentNullException(name);

    public static string AgainstNullOrWhiteSpace(string? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        => string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value must not be null or whitespace.", name)
            : value;

    public static decimal AgainstNegative(decimal value, [CallerArgumentExpression(nameof(value))] string? name = null)
        => value < 0m
            ? throw new ArgumentOutOfRangeException(name, value, "Value must not be negative.")
            : value;

    public static Guid AgainstEmpty(Guid value, [CallerArgumentExpression(nameof(value))] string? name = null)
        => value == Guid.Empty
            ? throw new ArgumentException("Value must not be an empty GUID.", name)
            : value;
}
