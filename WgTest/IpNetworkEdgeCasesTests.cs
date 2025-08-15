using System.Net.Sockets;
using WireGuardTools.ClassesOld;

namespace WgTest;

public class IpNetworkEdgeCasesTests
{
    [ Theory ]
    [ InlineData ( "0.0.0.0/0" ) ]
    [ InlineData ( "255.255.255.255/32" ) ]
    [ InlineData ( "::/0" ) ]
    [ InlineData ( "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff/128" ) ]
    public void Parse_EdgeCases_ShouldSucceed ( string cidr )
    {
        var network = IpNetwork.Parse ( cidr );
        Assert.NotNull ( network.NetworkAddress );
    }

    [ Fact ]
    public void Parse_IPv4MappedIPv6_ShouldWorkCorrectly()
    {
        var network = IpNetwork.Parse ( "::ffff:192.168.1.0/128" );
        Assert.Equal ( AddressFamily.InterNetworkV6 , network.AddressFamily );
    }

    [ Theory ]
    [ InlineData ( "192.168.1.0/31" ) ]
    [ InlineData ( "192.168.1.0/32" ) ]
    public void SpecialPrefixLengths_ShouldHandleCorrectly( string cidr )
    {
        var network = IpNetwork.Parse ( cidr );
        var hostCount = network.GetHostCount();
        Assert.True ( hostCount >= 0 );

        if ( network.AddressFamily == AddressFamily.InterNetwork ) {
            var firstUsable = network.FirstUsableAddress;
            var lastUsable = network.LastUsableAddress;
            var broadcast = network.BroadcastAddress;
            Assert.NotNull ( firstUsable );
            Assert.NotNull ( lastUsable );
            Assert.NotNull ( broadcast );
        }
    }

    [ Fact ]
    public void BroadcastAddress_EdgeCase_Slash31_ShouldWork()
    {
        var network = IpNetwork.Parse ( "192.168.1.0/31" );
        var broadcast = network.BroadcastAddress;
        Assert.Equal ( "192.168.1.1" , broadcast.ToString() );
    }

    [ Fact ]
    public void BroadcastAddress_EdgeCase_Slash32_ShouldWork()
    {
        var network = IpNetwork.Parse ( "192.168.1.5/32" );
        var broadcast = network.BroadcastAddress;
        Assert.Equal ( "192.168.1.5" , broadcast.ToString() );
    }

    [ Theory ]
    [ InlineData ( "224.0.0.0/4" , true ) ]
    [ InlineData ( "192.168.1.0/24" , false ) ]
    public void IsMulticast_IPv4_ShouldReturnCorrectResult( string cidr , bool expected )
    {
        var network = IpNetwork.Parse ( cidr );
        Assert.Equal ( expected , network.IsMulticast );
    }

    [ Theory ]
    [ InlineData ( "ff00::/8" , true ) ]
    [ InlineData ( "2001:db8::/32" , false ) ]
    public void IsMulticast_IPv6_ShouldReturnCorrectResult( string cidr , bool expected )
    {
        var network = IpNetwork.Parse ( cidr );
        Assert.Equal ( expected , network.IsMulticast );
    }

    [ Theory ]
    [ InlineData ( "169.254.1.0/24" , true ) ]
    [ InlineData ( "192.168.1.0/24" , false ) ]
    public void IsLinkLocal_IPv4_ShouldReturnCorrectResult( string cidr , bool expected )
    {
        var network = IpNetwork.Parse ( cidr );
        Assert.Equal ( expected , network.IsLinkLocal );
    }

    [ Theory ]
    [ InlineData ( "fe80::/10" , true ) ]
    [ InlineData ( "2001:db8::/32" , false ) ]
    public void IsLinkLocal_IPv6_ShouldReturnCorrectResult( string cidr , bool expected )
    {
        var network = IpNetwork.Parse ( cidr );
        Assert.Equal ( expected , network.IsLinkLocal );
    }
}