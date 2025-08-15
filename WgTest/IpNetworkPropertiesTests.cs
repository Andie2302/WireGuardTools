using WireGuardTools.ClassesOld;

namespace WgTest;

public class IpNetworkPropertiesTests
{
    [ Theory ]
    [ InlineData ( "192.168.1.0/24" , "255.255.255.0" ) ]
    [ InlineData ( "10.0.0.0/8" , "255.0.0.0" ) ]
    [ InlineData ( "172.16.0.0/12" , "255.240.0.0" ) ]
    [ InlineData ( "192.168.1.0/30" , "255.255.255.252" ) ]
    public void SubnetMask_IPv4_ShouldReturnCorrectMask( string networkCidr , string expectedMask )
    {
        var network = IpNetwork.Parse ( networkCidr );
        Assert.Equal ( expectedMask , network.SubnetMask.ToString() );
    }

    [ Fact ]
    public void SubnetMask_IPv6_ShouldThrowNotSupportedException()
    {
        var network = IpNetwork.Parse ( "2001:db8::/32" );
        Assert.Throws< NotSupportedException > ( () => network.SubnetMask );
    }

    [ Theory ]
    [ InlineData ( "192.168.1.0/24" , "192.168.1.255" ) ]
    [ InlineData ( "10.0.0.0/8" , "10.255.255.255" ) ]
    [ InlineData ( "172.16.0.0/12" , "172.31.255.255" ) ]
    public void BroadcastAddress_IPv4_ShouldReturnCorrectAddress( string networkCidr , string expectedBroadcast )
    {
        var network = IpNetwork.Parse ( networkCidr );
        Assert.Equal ( expectedBroadcast , network.BroadcastAddress.ToString() );
    }

    [ Theory ]
    [ InlineData ( "192.168.1.0/24" , "192.168.1.1" ) ]
    [ InlineData ( "10.0.0.0/8" , "10.0.0.1" ) ]
    [ InlineData ( "172.16.0.0/30" , "172.16.0.1" ) ]
    public void FirstUsableAddress_IPv4_ShouldReturnCorrectAddress( string networkCidr , string expectedFirst )
    {
        var network = IpNetwork.Parse ( networkCidr );
        Assert.Equal ( expectedFirst , network.FirstUsableAddress.ToString() );
    }

    [ Theory ]
    [ InlineData ( "192.168.1.0/24" , "192.168.1.254" ) ]
    [ InlineData ( "10.0.0.0/8" , "10.255.255.254" ) ]
    [ InlineData ( "172.16.0.0/30" , "172.16.0.2" ) ]
    public void LastUsableAddress_IPv4_ShouldReturnCorrectAddress( string networkCidr , string expectedLast )
    {
        var network = IpNetwork.Parse ( networkCidr );
        Assert.Equal ( expectedLast , network.LastUsableAddress.ToString() );
    }

    [ Theory ]
    [ InlineData ( "192.168.1.0/31" ) ]
    [ InlineData ( "192.168.1.1/32" ) ]
    public void FirstAndLastUsableAddress_SpecialCases_ShouldNotSubtractBroadcastAndNetwork( string networkCidr )
    {
        var network = IpNetwork.Parse ( networkCidr );
        Assert.NotNull ( network.FirstUsableAddress );
        Assert.NotNull ( network.LastUsableAddress );
    }

    [ Theory ]
    [ InlineData ( "10.0.0.0/8" , true ) ]
    [ InlineData ( "172.16.0.0/12" , true ) ]
    [ InlineData ( "192.168.0.0/16" , true ) ]
    [ InlineData ( "8.8.8.8/32" , false ) ]
    [ InlineData ( "1.1.1.1/32" , false ) ]
    public void IsPrivateNetwork_ShouldReturnCorrectResult( string networkCidr , bool expected )
    {
        var network = IpNetwork.Parse ( networkCidr );
        Assert.Equal ( expected , network.IsPrivateNetwork );
    }

    [ Theory ]
    [ InlineData ( "127.0.0.1/32" , true ) ]
    [ InlineData ( "::1/128" , true ) ]
    [ InlineData ( "192.168.1.1/32" , false ) ]
    public void IsLoopback_ShouldReturnCorrectResult( string networkCidr , bool expected )
    {
        var network = IpNetwork.Parse ( networkCidr );
        Assert.Equal ( expected , network.IsLoopback );
    }
}