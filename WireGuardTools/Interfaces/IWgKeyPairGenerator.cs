using WireGuardTools.Classes.Base;

namespace WireGuardTools.Interfaces;

public interface IWgKeyPairGenerator
{
    WgKeys GenerateKeyPair();
}