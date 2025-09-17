namespace WireGuardTools.test;

/// <summary>
/// Exception thrown when WireGuard tool operations fail
/// </summary>
public class WireGuardToolException : Exception
{
    public WireGuardToolException(string message) : base(message) { }
    public WireGuardToolException(string message, Exception innerException) : base(message, innerException) { }
}