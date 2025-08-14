namespace WireGuardTools.Classes;

public record WireGuardKeyPair
{
    public byte[] PrivateKey { get; }
    public byte[] PublicKey { get; }
    public string PrivateKeyBase64 { get; }
    public string PublicKeyBase64 { get; }

    private WireGuardKeyPair(byte[] privateKey, byte[] publicKey)
    {
        PrivateKey = privateKey;
        PublicKey = publicKey;
        PrivateKeyBase64 = Convert.ToBase64String(PrivateKey);
        PublicKeyBase64 = Convert.ToBase64String(PublicKey);
    }

    public static WireGuardKeyPair Create(byte[] privateKey, byte[] publicKey)
    {
        if (!CheckKeys.CheckKeyPair(privateKey, publicKey))
            throw new ArgumentException("Invalid key pair");

        return new WireGuardKeyPair(privateKey, publicKey);
    }

    // Nur Public Key für Peers (Server kennt nur Client Public Key)
    public WireGuardPeerKey ToPeerKey() => new(PublicKey);
}
