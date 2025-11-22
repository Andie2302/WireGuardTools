
namespace WireGuardTools;

/// <summary>
/// Base class for all connection types.
/// </summary>
public abstract record ConnectionSettings
{
    public required string Host { get; init; }
    public int Port { get; init; } = 22;
    public required string Username { get; init; }
}