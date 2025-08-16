using System.Net;

namespace WireGuardTools.Classes.Builders;

public static class WgTools
{
    public static IPAddress Create (this SubnetMask subnetMask, int prefixLength )
    {
        if ( prefixLength is < 0 or > 32 ) { throw new ArgumentOutOfRangeException ( nameof ( prefixLength ) , "Die Präfixlänge muss für IPv4 zwischen 0 und 32 liegen." ); }

        var maskValue = prefixLength == 0 ? 0 : 0xFFFFFFFF << 32 - prefixLength;
        var maskBytes = new byte[ 4 ];
        maskBytes[0] = (byte) ( maskValue >> 24 );
        maskBytes[1] = (byte) ( maskValue >> 16 );
        maskBytes[2] = (byte) ( maskValue >> 8 );
        maskBytes[3] = (byte) maskValue;

        return new IPAddress ( maskBytes );
    }
}