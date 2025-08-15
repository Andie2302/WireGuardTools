namespace WireGuardTools.Classes.Base;

public readonly record struct WgBaseKey
{
    public const int KeySize = 32;

    public WgBaseKey ( byte[] key )
    {
        ArgumentNullException.ThrowIfNull ( key );

        if ( key.Length != KeySize ) { throw new ArgumentException ( $"WireGuard keys must be {KeySize} bytes long." , nameof ( key ) ); }

        Key = key;
        KeyBase64 = Convert.ToBase64String ( key );
    }

    public string KeyBase64 { get; init; }
    public byte[] Key { get; init; }
    public static implicit operator WgBaseKey ( byte[] key ) => new WgBaseKey ( key );
    public static implicit operator byte[] ( WgBaseKey key ) => key.Key;

    public void Deconstruct ( out byte[] key , out string keyBase64 )
    {
        key = Key;
        keyBase64 = KeyBase64;
    }
}
