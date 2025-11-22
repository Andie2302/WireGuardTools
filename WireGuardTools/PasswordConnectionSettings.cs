namespace WireGuardTools;

/// <summary>
/// Connection settings using username and password authentication.
/// </summary>
public sealed record PasswordConnectionSettings : ConnectionSettings
{
    public required string Password { get; init; }
}