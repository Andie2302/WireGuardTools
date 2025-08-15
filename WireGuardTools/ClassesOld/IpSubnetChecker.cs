namespace WireGuardTools.ClassesOld;

public class IpSubnetChecker
{
    public static void Main ( string[] args )
    {
        try {
            Console.WriteLine ( "=== IPv4 Tests ===" );
            TestSubnet ( "192.168.1.0/24" , "192.168.1.50" , true );
            TestSubnet ( "192.168.1.0/24" , "10.0.0.1" , false );
            TestSubnet ( "172.16.0.0/16" , "172.16.2.15" , true );
            TestSubnet ( "172.17.0.0/16" , "172.16.2.15" , false );
            Console.WriteLine ( "\n=== IPv6 Tests ===" );
            TestSubnet ( "2001:db8::/32" , "2001:db8:1::1" , true );
            TestSubnet ( "2001:db8::/32" , "2001:db9:1::1" , false );
            Console.WriteLine ( "\n=== Erweiterte Features ===" );
            var network = IpNetwork.Parse ( "192.168.1.0/24" );
            Console.WriteLine ( $"Netzwerk: {network}" );
            Console.WriteLine ( $"Subnetzmaske: {network.SubnetMask}" );
            Console.WriteLine ( $"Broadcast-Adresse: {network.BroadcastAddress}" );
            Console.WriteLine ( $"Erste Host-Adresse: {network.FirstUsableAddress}" );
            Console.WriteLine ( $"Letzte Host-Adresse: {network.LastUsableAddress}" );
            Console.WriteLine ( $"Anzahl Hosts: {network.GetHostCount()}" );
            Console.WriteLine ( $"Ist privates Netzwerk: {network.IsPrivateNetwork}" );
            Console.WriteLine ( $"Ist Loopback: {network.IsLoopback}" );
            Console.WriteLine ( "\n=== Netzwerk-Overlap Tests ===" );
            var net1 = IpNetwork.Parse ( "192.168.1.0/24" );
            var net2 = IpNetwork.Parse ( "192.168.1.128/25" );
            Console.WriteLine ( $"{net1} enthält {net2}: {net1.Contains ( net2 )}" );
            Console.WriteLine ( $"{net1} überschneidet sich mit {net2}: {net1.Overlaps ( net2 )}" );
        }
        catch ( Exception ex ) { Console.WriteLine ( $"Fehler: {ex.Message}" ); }
    }

    private static void TestSubnet ( string cidrString , string ipString , bool expected )
    {
        try {
            var subnet = IpNetwork.Parse ( cidrString );
            var result = subnet.Contains ( ipString );
            var status = result == expected ? "✓" : "✗";
            Console.WriteLine ( $"{status} {ipString} in {cidrString}: {result}" );
        }
        catch ( Exception ex ) { Console.WriteLine ( $"✗ Fehler bei {cidrString}/{ipString}: {ex.Message}" ); }
    }
}