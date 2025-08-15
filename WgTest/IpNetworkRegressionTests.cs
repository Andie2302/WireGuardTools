using WireGuardTools.Scraps;

namespace WgTest;

public class IpNetworkRegressionTests
{
    [ Fact ]
    public void BroadcastAddress_BitwiseNegation_ShouldBeCorrect()
    {
        var network = IpNetwork.Parse ( "192.168.1.0/24" );
        var broadcast = network.BroadcastAddress;
        Assert.Equal ( "192.168.1.255" , broadcast.ToString() );
        var testCases = new[] { ( "192.168.1.0/25" , "192.168.1.127" ) , ( "192.168.1.0/26" , "192.168.1.63" ) , ( "192.168.1.0/27" , "192.168.1.31" ) , ( "192.168.1.0/28" , "192.168.1.15" ) };

        foreach ( var (cidr , expectedBroadcast) in testCases ) {
            var net = IpNetwork.Parse ( cidr );
            Assert.Equal ( expectedBroadcast , net.BroadcastAddress.ToString() );
        }
    }

    [ Fact ]
    public void NetworkNormalization_ShouldWorkCorrectly()
    {
        var testCases = new[] { ( "192.168.1.100/24" , "192.168.1.0/24" ) , ( "10.5.10.200/8" , "10.0.0.0/8" ) , ( "172.16.50.75/12" , "172.16.0.0/12" ) };

        foreach ( var (input , expected) in testCases ) {
            var network = IpNetwork.Parse ( input );
            Assert.Equal ( expected , network.ToString() );
        }
    }

    [ Fact ]
    public void IPv6_LongAddresses_ShouldParseCorrectly()
    {
        var longIPv6 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334/64";
        var network = IpNetwork.Parse ( longIPv6 );
        Assert.Equal ( "2001:db8:85a3::/64" , network.ToString() );
        Assert.Equal ( 64 , network.PrefixLength );
    }

    [ Theory ]
    [ InlineData ( "192.168.1.0/0" , 4294967294L ) ]
    [ InlineData ( "192.168.1.0/1" , 2147483646L ) ]
    [ InlineData ( "192.168.1.0/2" , 1073741822L ) ]
    public void GetHostCount_LargeNetworks_ShouldNotOverflow( string cidr , long expectedCount )
    {
        var network = IpNetwork.Parse ( cidr );
        var hostCount = network.GetHostCount();
        Assert.Equal ( expectedCount , hostCount );
    }
}