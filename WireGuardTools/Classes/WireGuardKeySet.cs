namespace WireGuardTools.Classes;

public record WireGuardKeySet
{
    public byte[] PrivateKey { get; }
    public byte[] PublicKey { get; }
    public byte[] PresharedKey { get; }
    public string PrivateKeyBase64 { get; }
    public string PublicKeyBase64 { get; }
    public string PresharedKeyBase64 { get; }

    private WireGuardKeySet ( byte[] privateKey , byte[] publicKey , byte[] presharedKey )
    {
        PrivateKey = privateKey;
        PublicKey = publicKey;
        PresharedKey = presharedKey;
        PrivateKeyBase64 = Convert.ToBase64String ( PrivateKey );
        PublicKeyBase64 = Convert.ToBase64String ( PublicKey );
        PresharedKeyBase64 = Convert.ToBase64String ( PresharedKey );
    }
}
