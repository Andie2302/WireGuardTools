using System.Net;
using System.Net.Sockets;

namespace WireGuardTools.ExtensionMethods;

public static class IpAddressExtensions
{
    public static bool IsValidIpV4SubnetMask ( this IPAddress address )
    {
        if ( address.AddressFamily != AddressFamily.InterNetwork ) { return false; }
        var addressBytes = address.GetAddressBytes();
        var maskValue = (uint) ( addressBytes[0] << 24 | addressBytes[1] << 16 | addressBytes[2] << 8 | addressBytes[3] );
        var invertedValue = ~maskValue;
        return ( invertedValue + 1 & invertedValue ) == 0;

    }
}
