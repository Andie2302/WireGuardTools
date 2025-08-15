using NSec.Cryptography;
using WireGuardTools.ClassesOld.Base;

namespace WireGuardTools.ClassesOld.Generators;

public static class WgKeyGenerator
{
    private static WgKeys GenerateKeys()
    {
        var keyCreationParameters = new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport };
        using var key = Key.Create ( SignatureAlgorithm.Ed25519 , keyCreationParameters );
        var privateKeyBytes = key.Export ( KeyBlobFormat.RawPrivateKey );
        var publicKeyBytes = key.PublicKey.Export ( KeyBlobFormat.RawPublicKey );

        return new WgKeys ( new WgBaseKey ( privateKeyBytes ) , new WgBaseKey ( publicKeyBytes ) );
    }

    private static WgBaseKey GeneratePresharedKey()
    {
        var presharedKeyBytes = new byte[ 32 ];
        System.Security.Cryptography.RandomNumberGenerator.Fill ( presharedKeyBytes );

        return new WgBaseKey ( presharedKeyBytes );
    }

    private static WgTunnelKeys GenerateTunnelKey ( WgKeys serverKeys ) => new WgTunnelKeys ( serverKeys , GenerateKeys() , GeneratePresharedKey() );

    public static List< WgTunnelKeys > GenerateTunnelKeys ( int count )
    {
        var tunnels = new List< WgTunnelKeys >();
        var serverKey = GenerateKeys();

        for ( var i = 0 ; i < count ; i++ ) { tunnels.Add ( GenerateTunnelKey ( serverKey ) ); }

        return tunnels;
    }
}
