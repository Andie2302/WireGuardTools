namespace WireGuardTools;

/// <summary>
/// Provides utility constants and methods for WireGuard-related operations.
/// </summary>
public static class WgTools
{
    /// <summary>
    /// Represents the size, in bytes, of keys used in the WireGuard cryptographic operations.
    /// </summary>
    /// <remarks>
    /// This constant defines the standard key length as 32 bytes. It is used to validate and enforce
    /// consistency across key generation, encryption, and decryption operations in the WireGuard implementation.
    /// </remarks>
    public const int KeySize = 32;
}