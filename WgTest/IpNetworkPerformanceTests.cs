using System.Net;
using WireGuardTools.ClassesOld;

namespace WgTest;

public class IpNetworkPerformanceTests
{
    [ Fact ]
    public void Parse_ManyNetworks_ShouldBeEfficient()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for ( var i = 0 ; i < 10000 ; i++ ) {
            var network = IpNetwork.Parse ( $"192.168.{i % 256}.0/24" );
            Assert.NotNull ( network.NetworkAddress );
        }

        stopwatch.Stop();
        Assert.True ( stopwatch.ElapsedMilliseconds < 1000 , $"Parsing took {stopwatch.ElapsedMilliseconds}ms, which is too slow" );
    }

    [ Fact ]
    public void Contains_ManyChecks_ShouldBeEfficient()
    {
        var network = IpNetwork.Parse ( "10.0.0.0/8" );
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for ( var i = 0 ; i < 10000 ; i++ ) {
            var ip = IPAddress.Parse ( $"10.{i % 256}.{i * 2 % 256}.{i * 3 % 256}" );
            Assert.True ( network.Contains ( ip ) );
        }

        stopwatch.Stop();
        Assert.True ( stopwatch.ElapsedMilliseconds < 500 , $"Contains checks took {stopwatch.ElapsedMilliseconds}ms, which is too slow" );
    }
}