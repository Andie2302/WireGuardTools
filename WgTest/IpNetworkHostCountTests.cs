using WireGuardTools.Scraps;

namespace WgTest;

public class IpNetworkHostCountTests
{
    [ Theory ]
    [ InlineData ( "192.168.1.0/24" , 254 ) ]
    [ InlineData ( "192.168.1.0/30" , 2 ) ]
    [ InlineData ( "192.168.1.0/31" , 2 ) ]
    [ InlineData ( "192.168.1.1/32" , 1 ) ]
    [ InlineData ( "10.0.0.0/8" , 16777214 ) ]
    public void GetHostCount_IPv4_ShouldReturnCorrectCount( string networkCidr , long expectedCount )
    {
        var network = IpNetwork.Parse ( networkCidr );
        Assert.Equal ( expectedCount , network.GetHostCount() );
    }

    [ Fact ]
    public void GetHostCount_LargeNetwork_ShouldHandleOverflow()
    {
        var network = IpNetwork.Parse ( "0.0.0.0/0" );
        Assert.Equal ( 4294967294L , network.GetHostCount() );
    }

    [ Fact ]
    public void GetHostCount_IPv6_ShouldReturnLargeNumber()
    {
        var network = IpNetwork.Parse ( "2001:db8::/64" );
        Assert.Equal ( long.MaxValue , network.GetHostCount() );
    }
}