using System.Security.Cryptography;

namespace WireGuardTools;

public class WgKey : IDisposable
{
    private readonly byte[] _key;
    private bool _disposed;

    public WgKey(byte[]? key = null)
    {
        _key = new byte[WgTools.KeySize];
        if (key?.Length != WgTools.KeySize)
            RandomNumberGenerator.Fill(_key);
        else
            Array.Copy(key, _key, WgTools.KeySize);
    }

    public static WgKey CreateRandom() => new();

    public byte[] Key
    {
        get
        {
            ThrowIfDisposed();
            return _key.ToArray();
        }
    }

    public int Size
    {
        get
        {
            ThrowIfDisposed();
            return _key.Length;
        }
    }

    public string Base64
    {
        get
        {
            ThrowIfDisposed();
            return Convert.ToBase64String(_key);
        }
    }

    public bool IsValid => !_disposed && _key.Length == WgTools.KeySize;

    public void Regenerate()
    {
        ThrowIfDisposed();
        RandomNumberGenerator.Fill(_key);
    }

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(WgKey));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Array.Clear(_key, 0, _key.Length);
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
    public override string ToString() => Base64;
}