using WireGuardTools.ClassesOld;

namespace WgTest;

public class IpNetworkEqualityTests
{
    [ Fact ]
    public void Equals_SameNetworkAndPrefix_ShouldReturnTrue()
    {
        var network1 = IpNetwork.Parse ( "192.168.1.0/24" );
        var network2 = IpNetwork.Parse ( "192.168.1.0/24" );
        Assert.True ( network1.Equals ( network2 ) );
        Assert.True ( network1 == network2 );
        Assert.False ( network1 != network2 );
    }

    [ Fact ]
    public void Equals_DifferentNetwork_ShouldReturnFalse()
    {
        var network1 = IpNetwork.Parse ( "192.168.1.0/24" );
        var network2 = IpNetwork.Parse ( "192.168.2.0/24" );
        Assert.False ( network1.Equals ( network2 ) );
        Assert.False ( network1 == network2 );
        Assert.True ( network1 != network2 );
    }

    [ Fact ]
    public void Equals_DifferentPrefix_ShouldReturnFalse()
    {
        var network1 = IpNetwork.Parse ( "192.168.1.0/24" );
        var network2 = IpNetwork.Parse ( "192.168.1.0/25" );
        Assert.False ( network1.Equals ( network2 ) );
    }

    [ Fact ]
    public void GetHashCode_SameNetworks_ShouldReturnSameHashCode()
    {
        var network1 = IpNetwork.Parse ( "192.168.1.0/24" );
        var network2 = IpNetwork.Parse ( "192.168.1.0/24" );
        Assert.Equal ( network1.GetHashCode() , network2.GetHashCode() );
    }

    [ Fact ]
    public void ToString_ShouldReturnCorrectFormat()
    {
        var network = IpNetwork.Parse ( "192.168.1.0/24" );
        Assert.Equal ( "192.168.1.0/24" , network.ToString() );
    }
}