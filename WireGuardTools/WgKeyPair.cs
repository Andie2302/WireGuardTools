namespace WireGuardTools;

public class WgKeyPair : IDisposable
{
    private readonly IWgKeyPairGenerator _generator;

    public WgKey PrivateKey { get; private set; }
    public WgKey PublicKey { get; private set; }

    // Option A: Komplett generieren
    public WgKeyPair(IWgKeyPairGenerator generator)
    {
        _generator = generator;
        var keyPair = generator.GenerateKeyPair();
        PrivateKey = keyPair.PrivateKey;
        PublicKey = keyPair.PublicKey;
    }

    // Option C: Aus Private Key ableiten
    public WgKeyPair(WgKey privateKey, IWgKeyPairGenerator generator)
    {
        _generator = generator;
        PrivateKey = privateKey;
        PublicKey = generator.GeneratePublicKeyFromPrivate(privateKey);
    }

    // Factory-Methoden für Convenience
    public static WgKeyPair Generate() => new(new Curve25519KeyPairGenerator());
    public static WgKeyPair FromPrivateKey(WgKey privateKey) => new(privateKey, new Curve25519KeyPairGenerator());

    public void Dispose()
    {
        PrivateKey.Dispose();
        PublicKey.Dispose();
    }
}