using System.Security.Cryptography;

namespace WireGuardTools;

public readonly record struct WgPreSharedKey
{
    public byte[] Key { get; }
    public string KeyAsBase64 => Convert.ToBase64String(this.Key);
    public WgPreSharedKey(byte[] key)
    {
        this.Key = key;
    }

    public WgPreSharedKey()
    {
        this.Key = new byte[WgTools.KeySize];
        RandomNumberGenerator.Fill(this.Key);
    }
}