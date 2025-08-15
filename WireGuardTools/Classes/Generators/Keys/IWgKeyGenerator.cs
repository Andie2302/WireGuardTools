using WireGuardTools.Classes.Base;

namespace WireGuardTools.Classes.Generators.Keys;

public interface IWgKeyGenerator
{
    WgKeys GenerateKeyPair();
    WgBaseKey GeneratePresharedKey();
}
