namespace WireGuardTools.Scraps;

public class Program
{
    public static void Main()
    {
        Console.WriteLine ( "=== SubnetMask Tests ===" );
        var mask1 = SubnetMask.FromPrefixLength ( 24 );
        var mask2 = SubnetMask.FromMaskString ( "255.255.255.0" );
        SubnetMask mask3 = 24;
        SubnetMask mask4 = "255.255.255.0";
        Console.WriteLine ( $"Alle Masken sind gleich: {mask1 == mask2 && mask2.Equals ( mask3 ) && mask3.Equals ( mask4 )}" );
        Console.WriteLine ( $"Maske /24: {mask1} ({mask1.ToCidrString()})" );
        Console.WriteLine ( "\n=== IPNetwork Tests ===" );
        var network = new IpNetwork ( "192.168.1.100" , mask1 );
        Console.WriteLine ( network.ToDetailedString() );
        var network2 = IpNetwork.Parse ( "10.0.0.0/16" );
        Console.WriteLine ( $"\nNetzwerk aus CIDR: {network2}" );
        Console.WriteLine ( $"Enthält 10.0.5.1: {network2.Contains ( "10.0.5.1" )}" );
        Console.WriteLine ( $"Enthält 11.0.0.1: {network2.Contains ( "11.0.0.1" )}" );
        Console.WriteLine ( "\n=== Navigation Tests ===" );
        var smallNetwork = IpNetwork.Parse ( "192.168.1.0/30" );
        Console.WriteLine ( $"Kleines Netzwerk: {smallNetwork.ToDetailedString()}" );
        var currentIp = smallNetwork.NetworkAddress;
        Console.WriteLine ( $"\nNavigation durch {smallNetwork}:" );

        for ( var i = 0 ; i < 6 ; i++ ) {
            Console.WriteLine ( $"  {i + 1}. Adresse: {currentIp}" );
            currentIp = smallNetwork.GetNextAddress ( currentIp );
        }

        Console.WriteLine ( "\nAlle nutzbaren Adressen in /28 Netzwerk:" );
        var mediumNetwork = IpNetwork.Parse ( "192.168.1.0/28" );

        foreach ( var addr in mediumNetwork.GetUsableAddresses().Take ( 5 ) ) { Console.WriteLine ( $"  {addr}" ); }

        Console.WriteLine ( $"  ... und {mediumNetwork.UsableAddressCount - 5} weitere" );
    }
}