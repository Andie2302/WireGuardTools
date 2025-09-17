namespace WireGuardTools;

public class WgKey : IDisposable
{
    private readonly byte[] _key;

    public WgKey(byte[] key)
    {
        _key = key;
        Check();
    }

    public byte[] Key => _key.ToArray();
    public int Size => _key.Length;
    public string Base64 => Convert.ToBase64String(_key);
    public void Clear() => Array.Clear(_key, 0, _key.Length);

    public void Check()
    {
        if (Size != WgTools.KeySize)
            throw new ArgumentException($"Key must be {WgTools.KeySize} bytes long", nameof(_key));
    }

    public void Dispose() => Clear();
}

public static class WgTools
{
    public const int KeySize = 32;
}