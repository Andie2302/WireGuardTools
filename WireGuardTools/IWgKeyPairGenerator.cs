namespace WireGuardTools;

public interface IWgKeyPairGenerator
{
    WgKeyPair GenerateKeyPair();
}
public interface ICustomWgKeyPairGenerator
{
    WgKey GeneratePublicKeyFromPrivate(WgKey privateKey);
}
