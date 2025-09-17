namespace WireGuardTools;

public static WgKeyPair Generate() => new(new Curve25519KeyPairGenerator());
public static WgKeyPair FromPrivateKey(WgKey privateKey) => new(privateKey, new Curve25519KeyPairGenerator());



public class Curve25519KeyPairGenerator : IWgKeyPairGenerator
{

    public WgKey GeneratePublicKeyFromPrivate(WgKey privateKey)
    {
        // Curve25519 Magic hier
        byte[] publicKeyBytes = GeneratePublicKeyFromPrivate(privateKey.Key);
        return new WgKey(publicKeyBytes);
    }

    public WgKeyPair GenerateKeyPair()
    {
        var privateKey = WgKey.CreateRandom();
        var publicKey = GeneratePublicKeyFromPrivate(privateKey);
        return new WgKeyPair(privateKey, publicKey);
    }
}