using NSec.Cryptography;
using WireGuardTools.Classes.Base;

namespace WireGuardTools.Classes.Generators;

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

    public static WgTunnel GenerateTunnelKeys() => new WgTunnel ( GenerateKeys() , GenerateKeys() , GeneratePresharedKey() );
    
}
