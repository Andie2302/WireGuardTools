using System.Security.Cryptography;

namespace WireGuardTools;

public class Curve25519KeyPairGenerator : IWgKeyPairGenerator
{
    public WgKeyPair GenerateKeyPair()
    {
        using var ecdh = ECDiffieHellman.Create(WgCurve.Curve25519);
        var ecParameters = ecdh.ExportParameters(true);
        if (ecParameters.D is null) throw new Exception("D is null");
        if (ecParameters.Q.X is null) throw new Exception("Q.X is null");
        return ecParameters.Q.Y is null ? throw new Exception("Q.Y is null") : new WgKeyPair(new WgKey(ecParameters.D), new WgKey(ecParameters.Q.X));
    }
}