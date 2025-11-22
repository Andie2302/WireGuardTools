namespace WireGuardTools;

/// <summary>
/// Connection settings using public/private key authentication.
/// </summary>
public sealed record KeyConnectionSettings : ConnectionSettings
{
    public required string PrivateKeyPath { get; init; }
    public string Passphrase { get; init; } = string.Empty;
}