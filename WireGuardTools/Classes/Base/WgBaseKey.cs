namespace WireGuardTools.ClassesOld.Base;

public readonly record struct WgBaseKey ( byte[] Key )
{
    public string KeyBase64 { get; init; } = Convert.ToBase64String ( Key );

    public void Deconstruct ( out byte[] key , out string keyBase64 )
    {
        key = Key;
        keyBase64 = KeyBase64;
    }

    public static implicit operator WgBaseKey ( byte[] key ) => new WgBaseKey ( key );
    public static implicit operator byte[] ( WgBaseKey key ) => key.Key;
}