// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace WireGuardTools;

public class WgKeyPair : IDisposable
{
    public WgKey PrivateKey { get; private set; }
    public WgKey PublicKey { get; private set; }
    public WgKeyPair(WgKey privateKey, WgKey publicKey)
    {
        PrivateKey = privateKey;
        PublicKey = publicKey;
    }

    public WgKeyPair(IWgKeyPairGenerator generator)
    {
        var keyPair = generator.GenerateKeyPair();
        PrivateKey = keyPair.PrivateKey;
        PublicKey = keyPair.PublicKey;
    }
    public WgKeyPair(IWgFromPrivateKeyKeyPairGenerator generator, WgKey privateKey)
    {
        var keyPair = generator.GenerateKeyPair(privateKey);
        PrivateKey = keyPair.PrivateKey;
        PublicKey = keyPair.PublicKey;
    }

    public void Dispose()
    {
        PrivateKey.Dispose();
        PublicKey.Dispose();
    }
    public override string ToString() => $"{PrivateKey} {PublicKey}";
}