namespace WireGuardTools;

public interface IWgKeyPairGenerator
{
    WgKeyPair GenerateKeyPair();
}
public interface IWgFromPrivateKeyKeyPairGenerator : IWgKeyPairGenerator
{
    WgKeyPair GenerateKeyPair(WgKey privateKey);
}
