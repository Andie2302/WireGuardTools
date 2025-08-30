using System.Security.Cryptography;
using WireGuardTools.Generators;

namespace WireGuardTools;

/// <summary>
/// Represents a WireGuard compatible Curve25519 key pair.
/// This type is immutable and performs validation on creation.
/// </summary>
public readonly record struct WgPeer
{
    /// <summary>
    /// Gets the raw private key (32 bytes).
    /// </summary>
    public byte[] PrivateKey { get; }

    /// <summary>
    /// Gets the raw public key (32 bytes).
    /// </summary>
    public byte[] PublicKey { get; }

    /// <summary>
    /// Gets the private key as a Base64 encoded string.
    /// </summary>
    public string PrivateKeyAsBase64 => Convert.ToBase64String(this.PrivateKey);

    /// <summary>
    /// Gets the public key as a Base64 encoded string.
    /// </summary>
    public string PublicKeyAsBase64 => Convert.ToBase64String(this.PublicKey);

    /// <summary>
    /// Initializes a new instance of the <see cref="WgPeer"/> struct with validation.
    /// </summary>
    /// <param name="privateKey">The 32-byte private key.</param>
    /// <param name="publicKey">The 32-byte public key.</param>
    public WgPeer(byte[] privateKey, byte[] publicKey)
    {
        ArgumentNullException.ThrowIfNull(privateKey);

        ArgumentNullException.ThrowIfNull(publicKey);

        if (privateKey.Length != WgTools.KeySize)
        {
            throw new ArgumentException($"Private key must be {WgTools.KeySize} bytes long.", nameof(privateKey));
        }

        if (publicKey.Length != WgTools.KeySize)
        {
            throw new ArgumentException($"Public key must be {WgTools.KeySize} bytes long.", nameof(publicKey));
        }

        this.PrivateKey = privateKey;
        this.PublicKey = publicKey;
    }

    public static WgPeer CreateRandom() => WgPeerGenerator.Create();
}