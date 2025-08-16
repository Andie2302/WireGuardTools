using WireGuardTools.Classes.Base;

namespace WireGuardTools.Interfaces.Generators;

public interface IWgKeyPairGenerator
{
    WgKeys GenerateKeyPair();
}