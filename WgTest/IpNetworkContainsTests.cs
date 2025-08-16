using System.Net;

namespace WgTest;

public class IpNetworkContainsTests
{
    [ Theory ]
    [ InlineData ( "192.168.1.0/24" , "192.168.1.1" , true ) ]
    [ InlineData ( "192.168.1.0/24" , "192.168.1.254" , true ) ]
    [ InlineData ( "192.168.1.0/24" , "192.168.2.1" , false ) ]
    [ InlineData ( "10.0.0.0/8" , "10.255.255.255" , true ) ]
    [ InlineData ( "10.0.0.0/8" , "11.0.0.1" , false ) ]
    [ InlineData ( "172.16.0.0/12" , "172.31.255.255" , true ) ]
    [ InlineData ( "172.16.0.0/12" , "172.32.0.1" , false ) ]
    public void Contains_IPv4_ShouldReturnCorrectResult ( string networkCidr , string ipAddress , bool expected )
    {
        var network = IpNetwork.Parse ( networkCidr );
        var ip = IPAddress.Parse ( ipAddress );
        Assert.Equal ( expected , network.Contains ( ip ) );
        Assert.Equal ( expected , network.Contains ( ipAddress ) );
    }

    [ Theory ]
    [ InlineData ( "2001:db8::/32" , "2001:db8:1::1" , true ) ]
    [ InlineData ( "2001:db8::/32" , "2001:db9::1" , false ) ]
    [ InlineData ( "::1/128" , "::1" , true ) ]
    [ InlineData ( "::1/128" , "::2" , false ) ]
    public void Contains_IPv6_ShouldReturnCorrectResult ( string networkCidr , string ipAddress , bool expected )
    {
        var network = IpNetwork.Parse ( networkCidr );
        var ip = IPAddress.Parse ( ipAddress );
        Assert.Equal ( expected , network.Contains ( ip ) );
        Assert.Equal ( expected , network.Contains ( ipAddress ) );
    }

    [ Fact ]
    public void Contains_NullIPAddress_ShouldReturnFalse()
    {
        var network = IpNetwork.Parse ( "192.168.1.0/24" );
        Assert.False ( network.Contains ( (IPAddress) null ) );
    }

    [ Fact ]
    public void Contains_InvalidIPString_ShouldReturnFalse()
    {
        var network = IpNetwork.Parse ( "192.168.1.0/24" );
        Assert.False ( network.Contains ( "invalid.ip.address" ) );
    }

    [ Fact ]
    public void Contains_DifferentAddressFamilies_ShouldReturnFalse()
    {
        var ipv4Network = IpNetwork.Parse ( "192.168.1.0/24" );
        var ipv6Address = IPAddress.Parse ( "2001:db8::1" );
        Assert.False ( ipv4Network.Contains ( ipv6Address ) );
    }
}