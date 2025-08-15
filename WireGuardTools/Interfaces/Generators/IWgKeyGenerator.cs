using WireGuardTools.Classes.Base;

namespace WireGuardTools.Interfaces.Generators;

public interface IWgKeyGenerator
{
    WgKeys GenerateKeyPair();
    WgBaseKey GeneratePresharedKey();
}
