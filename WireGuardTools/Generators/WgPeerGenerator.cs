using System.Security.Cryptography;
using WireGuardTools.Generators.Constants;

namespace WireGuardTools.Generators;

/// <summary>
/// Provides functionality for generating WireGuard compatible Curve25519 key pairs.
/// </summary>
public class WgPeerGenerator
{
    /// <summary>
    /// Generates a new WireGuard compatible key pair using Curve25519.
    /// </summary>
    /// <returns>
    /// A <see cref="WgPeer"/> object containing the generated private key and public key.
    /// </returns>
    public static WgPeer Create()
    {
        var publicKeyBytes = new byte[WgTools.KeySize];
        byte[] privateKeyBytes;
        using (var ecdh = ECDiffieHellman.Create(WgCurve25519Constants.Curve25519))
        {
            var keyParameters = ecdh.ExportParameters(true);

            if (keyParameters.D == null || keyParameters.Q.X == null)
            {
                throw new InvalidOperationException("Failed to generate valid key parameters");
            }

            privateKeyBytes = keyParameters.D;

            var sourceLength = Math.Min(WgTools.KeySize, keyParameters.Q.X.Length);
            Array.Copy(keyParameters.Q.X, 0, publicKeyBytes, 0, sourceLength);

            if (sourceLength < WgTools.KeySize)
            {
                Array.Fill<byte>(publicKeyBytes, 0, sourceLength, WgTools.KeySize - sourceLength);
            }
        }

        return new WgPeer(privateKeyBytes, publicKeyBytes);
    }

    /// <summary>
    /// Generates multiple WireGuard key pairs.
    /// </summary>
    /// <param name="count">The number of key pairs to generate.</param>
    /// <returns>An enumerable collection of <see cref="WgPeer"/> objects.</returns>
    public static IEnumerable<WgPeer> Create(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return Create();
        }
    }
}