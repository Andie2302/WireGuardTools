namespace WireGuardTools;

public interface IWgFromPrivateKeyKeyPairGenerator : IWgKeyPairGenerator
{
    WgKeyPair GenerateKeyPair(WgKey privateKey);
}