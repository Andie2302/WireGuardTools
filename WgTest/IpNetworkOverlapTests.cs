using WireGuardTools.Scraps;

namespace WgTest;

public class IpNetworkOverlapTests
{
    [ Theory ]
    [ InlineData ( "192.168.1.0/24" , "192.168.1.128/25" , true ) ]
    [ InlineData ( "192.168.1.0/25" , "192.168.1.0/24" , false ) ]
    [ InlineData ( "192.168.1.0/24" , "192.168.2.0/24" , false ) ]
    [ InlineData ( "10.0.0.0/8" , "10.1.1.0/24" , true ) ]
    public void Contains_Network_ShouldReturnCorrectResult( string network1Cidr , string network2Cidr , bool expected )
    {
        var network1 = IpNetwork.Parse ( network1Cidr );
        var network2 = IpNetwork.Parse ( network2Cidr );
        Assert.Equal ( expected , network1.Contains ( network2 ) );
    }

    [ Theory ]
    [ InlineData ( "192.168.1.0/24" , "192.168.1.128/25" , true ) ]
    [ InlineData ( "192.168.1.0/24" , "192.168.2.0/24" , false ) ]
    [ InlineData ( "10.0.0.0/16" , "10.1.0.0/16" , true ) ]
    public void Overlaps_ShouldReturnCorrectResult( string network1Cidr , string network2Cidr , bool expected )
    {
        var network1 = IpNetwork.Parse ( network1Cidr );
        var network2 = IpNetwork.Parse ( network2Cidr );
        Assert.Equal ( expected , network1.Overlaps ( network2 ) );
        Assert.Equal ( expected , network2.Overlaps ( network1 ) );
    }

    [ Fact ]
    public void Contains_DifferentAddressFamilies_ShouldReturnFalse()
    {
        var ipv4Network = IpNetwork.Parse ( "192.168.1.0/24" );
        var ipv6Network = IpNetwork.Parse ( "2001:db8::/32" );
        Assert.False ( ipv4Network.Contains ( ipv6Network ) );
        Assert.False ( ipv4Network.Overlaps ( ipv6Network ) );
    }
}