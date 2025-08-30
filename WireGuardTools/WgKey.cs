using System.Security.Cryptography;

namespace WireGuardTools;

/// <summary>
/// Represents a WireGuard compatible 32-byte cryptographic key.
/// This type is immutable and performs validation on creation.
/// </summary>
public readonly record struct WgKey
{
    /// <summary>
    /// Gets the raw key bytes (32 bytes).
    /// </summary>
    public byte[] Key { get; }

    /// <summary>
    /// Gets the key as a Base64 encoded string.
    /// </summary>
    public string KeyAsBase64 => Convert.ToBase64String(this.Key);

    /// <summary>
    /// Private constructor that performs validation on the key bytes.
    /// </summary>
    /// <param name="key">The 32-byte key.</param>
    /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
    /// <exception cref="ArgumentException">Thrown when key is not exactly 32 bytes.</exception>
    private WgKey(byte[] key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (key.Length != WgTools.KeySize)
        {
            throw new ArgumentException($"Key must be exactly {WgTools.KeySize} bytes long, but was {key.Length} bytes.", nameof(key));
        }

        // Create a defensive copy to ensure immutability
        this.Key = new byte[WgTools.KeySize];
        Array.Copy(key, this.Key, WgTools.KeySize);
    }

    /// <summary>
    /// Creates a new WgKey from the provided byte array.
    /// </summary>
    /// <param name="key">The 32-byte key.</param>
    /// <returns>A new WgKey instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
    /// <exception cref="ArgumentException">Thrown when key is not exactly 32 bytes.</exception>
    public static WgKey Create(byte[] key) => new WgKey(key);

    /// <summary>
    /// Creates a new WgKey from a Base64 encoded string.
    /// </summary>
    /// <param name="base64Key">The Base64 encoded key string.</param>
    /// <returns>A new WgKey instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when base64Key is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the decoded key is not exactly 32 bytes.</exception>
    /// <exception cref="FormatException">Thrown when base64Key is not a valid Base64 string.</exception>
    public static WgKey Create(string base64Key)
    {
        ArgumentNullException.ThrowIfNull(base64Key);

        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(base64Key);
        }
        catch (FormatException ex)
        {
            throw new FormatException($"Invalid Base64 key format: {base64Key}", ex);
        }

        return new WgKey(keyBytes);
    }

    /// <summary>
    /// Creates a new WgKey with a cryptographically secure random 32-byte key.
    /// </summary>
    /// <returns>A new WgKey instance with a randomly generated key.</returns>
    public static WgKey CreateRandom()
    {
        var keyBytes = new byte[WgTools.KeySize];
        RandomNumberGenerator.Fill(keyBytes);
        return new WgKey(keyBytes);
    }

    /// <summary>
    /// Returns a string representation suitable for logging.
    /// Shows the first 8 characters of the Base64 key followed by "..." for security.
    /// </summary>
    /// <returns>A truncated representation of the key for logging purposes.</returns>
    public override string ToString()
    {
        var base64 = this.KeyAsBase64;
        if (base64.Length <= 8)
        {
            return $"WgKey[{base64}...]";
        }
        return $"WgKey[{base64[..8]}...]";
    }

    /// <summary>
    /// Returns the full Base64 representation of the key.
    /// Use with caution in production environments.
    /// </summary>
    /// <returns>The complete Base64 encoded key.</returns>
    public string ToFullString()
    {
        return $"WgKey[{this.KeyAsBase64}]";
    }

    /// <summary>
    /// Compares two WgKey instances for equality by comparing their key bytes.
    /// </summary>
    /// <param name="other">The other WgKey to compare with.</param>
    /// <returns>True if the keys are equal, false otherwise.</returns>
    public bool Equals(WgKey other)
    {
        return this.Key.SequenceEqual(other.Key);
    }

    /// <summary>
    /// Gets the hash code for this WgKey based on its key bytes.
    /// </summary>
    /// <returns>A hash code for the current WgKey.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var b in this.Key)
        {
            hash.Add(b);
        }
        return hash.ToHashCode();
    }
}