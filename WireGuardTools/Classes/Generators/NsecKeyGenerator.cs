using NSec.Cryptography;
using WireGuardTools.Classes.Base;
using WireGuardTools.Classes.Static;
using WireGuardTools.Interfaces.Generators;

namespace WireGuardTools.Classes.Generators;

public sealed class NsecKeyGenerator ( IRandomGenerator randomGenerator ) : IWgKeyGenerator
{
    public WgKeys GenerateKeyPair()
    {
        var keyCreationParameters = new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport };
        using var key = Key.Create ( SignatureAlgorithm.Ed25519 , keyCreationParameters );
        var privateKeyBytes = key.Export ( KeyBlobFormat.RawPrivateKey );
        var publicKeyBytes = key.PublicKey.Export ( KeyBlobFormat.RawPublicKey );

        return new WgKeys ( new WgBaseKey ( privateKeyBytes ) , new WgBaseKey ( publicKeyBytes ) );
    }
    public IRandomGenerator RandomGenerator { get; init; } = randomGenerator;
    public WgBaseKey GeneratePresharedKey() => new WgBaseKey ( RandomGenerator.GetBytes ( WgConstants.KeySize ) );
}
