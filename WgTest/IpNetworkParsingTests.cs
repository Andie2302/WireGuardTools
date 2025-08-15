using WireGuardTools.ClassesOld;

namespace WgTest
{
    public class IpNetworkParsingTests
    {
        [ Theory ]
        [ InlineData ( "192.168.1.0/24" ) ]
        [ InlineData ( "10.0.0.0/8" ) ]
        [ InlineData ( "172.16.0.0/16" ) ]
        [ InlineData ( "2001:db8::/32" ) ]
        [ InlineData ( "::1/128" ) ]
        public void Parse_ValidCidr_ShouldSucceed ( string cidr )
        {
            var result = IpNetwork.Parse ( cidr );
            Assert.NotNull ( result.NetworkAddress );
        }

        [ Theory ]
        [ InlineData ( "" ) ]
        [ InlineData ( null ) ]
        [ InlineData ( "192.168.1.0" ) ]
        [ InlineData ( "192.168.1.0/" ) ]
        [ InlineData ( "/24" ) ]
        [ InlineData ( "192.168.1.0/33" ) ]
        [ InlineData ( "192.168.1.0/-1" ) ]
        [ InlineData ( "invalid.ip/24" ) ]
        [ InlineData ( "192.168.1.0/abc" ) ]
        public void Parse_InvalidCidr_ShouldThrowArgumentException ( string cidr ) { Assert.Throws< ArgumentException > ( () => IpNetwork.Parse ( cidr ) ); }

        [ Fact ]
        public void Parse_IPv4_ShouldNormalizeNetworkAddress()
        {
            var network = IpNetwork.Parse ( "192.168.1.100/24" );
            Assert.Equal ( "192.168.1.0" , network.NetworkAddress.ToString() );
            Assert.Equal ( 24 , network.PrefixLength );
        }

        [ Fact ]
        public void Parse_IPv6_ShouldNormalizeNetworkAddress()
        {
            var network = IpNetwork.Parse ( "2001:db8:1234:5678::abcd/64" );
            Assert.Equal ( "2001:db8:1234:5678::" , network.NetworkAddress.ToString() );
            Assert.Equal ( 64 , network.PrefixLength );
        }

        [ Theory ]
        [ InlineData ( "192.168.1.0/24" , true ) ]
        [ InlineData ( "invalid/format" , false ) ]
        [ InlineData ( "192.168.1.0/33" , false ) ]
        [ InlineData ( "" , false ) ]
        [ InlineData ( null , false ) ]
        public void TryParse_ShouldReturnCorrectResult ( string cidr , bool expectedResult )
        {
            var result = IpNetwork.TryParse ( cidr , out var network );
            Assert.Equal ( expectedResult , result );

            if ( expectedResult ) { Assert.NotNull ( network.NetworkAddress ); }
        }
    }
}
