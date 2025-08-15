namespace WireGuardTools.Classes.Static;

public static class WgUtility
{
    public static byte[] CreateKeySizedArray() => new byte[ WgConstants.KeySize ];
}