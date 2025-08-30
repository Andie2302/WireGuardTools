using System.Security.Cryptography;
using WireGuardTools.Generators.Constants;

namespace WireGuardTools.Generators;

/// <summary>
/// Provides functionality for generating WireGuard compatible Curve25519 key pairs.
/// </summary>
public class WgKeyPairGenerator
{
    /// <summary>
    /// Generates a new WireGuard compatible key pair using Curve25519.
    /// </summary>
    /// <returns>
    /// A <see cref="WgKeyPair"/> object containing the generated private key and public key.
    /// </returns>
    public static WgKeyPair CreateNewWgKeyPair()
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

        return new WgKeyPair(privateKeyBytes, publicKeyBytes);
    }

    /// <summary>
    /// Converts a given byte array key to its Base64 string representation.
    /// </summary>
    /// <param name="key">The byte array representing the key to be converted.</param>
    /// <returns>A Base64 encoded string representation of the provided key.</returns>
    public static string KeyToBase64(byte[] key) => Convert.ToBase64String(key);

    /// <summary>
    /// Generates multiple WireGuard key pairs.
    /// </summary>
    /// <param name="count">The number of key pairs to generate.</param>
    /// <returns>An enumerable collection of <see cref="WgKeyPair"/> objects.</returns>
    public static IEnumerable<WgKeyPair> CreateMultipleKeyPairs(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return CreateNewWgKeyPair();
        }
    }
}