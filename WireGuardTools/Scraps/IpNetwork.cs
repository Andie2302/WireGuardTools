using System.Net;
using System.Net.Sockets;

namespace WireGuardTools.Scraps;

public readonly struct IpNetwork : IEquatable< IpNetwork >
{
    public IPAddress NetworkAddress { get; }
    public int PrefixLength { get; }
    public AddressFamily AddressFamily => NetworkAddress?.AddressFamily ?? AddressFamily.Unspecified;
    public IPAddress SubnetMask
    {
        get
        {
            if ( AddressFamily != AddressFamily.InterNetwork ) { throw new NotSupportedException ( "Subnetzmaske ist nur für IPv4-Adressen verfügbar." ); }

            var mask = PrefixLength == 0 ? 0 : 0xFFFFFFFF << 32 - PrefixLength;

            return new IPAddress ( new byte[] { (byte) ( mask >> 24 ) , (byte) ( mask >> 16 ) , (byte) ( mask >> 8 ) , (byte) mask } );
        }
    }
    public IPAddress BroadcastAddress
    {
        get
        {
            if ( AddressFamily != AddressFamily.InterNetwork ) { throw new NotSupportedException ( "Broadcast-Adresse ist nur für IPv4-Adressen verfügbar." ); }

            var networkBytes = NetworkAddress.GetAddressBytes();
            var maskBytes = GetSubnetMaskBytes();
            var broadcastBytes = new byte[ networkBytes.Length ];

            for ( var i = 0 ; i < networkBytes.Length ; i++ ) { broadcastBytes[i] = (byte) ( networkBytes[i] | ~maskBytes[i] & 0xFF ); }

            return new IPAddress ( broadcastBytes );
        }
    }
    public IPAddress FirstUsableAddress
    {
        get
        {
            if ( AddressFamily != AddressFamily.InterNetwork ) { throw new NotSupportedException ( "FirstUsableAddress ist nur für IPv4-Adressen verfügbar." ); }

            if ( PrefixLength >= 31 ) { return NetworkAddress; }

            var addressBytes = NetworkAddress.GetAddressBytes();
            var address = BitConverter.ToUInt32 ( addressBytes.Reverse().ToArray() , 0 );
            address += 1;
            var resultBytes = BitConverter.GetBytes ( address ).Reverse().ToArray();

            return new IPAddress ( resultBytes );
        }
    }
    public IPAddress LastUsableAddress
    {
        get
        {
            if ( AddressFamily != AddressFamily.InterNetwork ) { throw new NotSupportedException ( "LastUsableAddress ist nur für IPv4-Adressen verfügbar." ); }

            if ( PrefixLength >= 31 ) { return BroadcastAddress; }

            var broadcastBytes = BroadcastAddress.GetAddressBytes();
            var address = BitConverter.ToUInt32 ( broadcastBytes.Reverse().ToArray() , 0 );
            address -= 1;
            var resultBytes = BitConverter.GetBytes ( address ).Reverse().ToArray();

            return new IPAddress ( resultBytes );
        }
    }
    public bool IsPrivateNetwork
    {
        get
        {
            if ( AddressFamily != AddressFamily.InterNetwork ) { return false; }

            return Contains ( "10.0.0.0" ) && PrefixLength >= 8 || Contains ( "172.16.0.0" ) && PrefixLength >= 12 || Contains ( "192.168.0.0" ) && PrefixLength >= 16;
        }
    }
    public bool IsLoopback => NetworkAddress != null && IPAddress.IsLoopback ( NetworkAddress );
    public bool IsMulticast => NetworkAddress != null && ( AddressFamily == AddressFamily.InterNetwork ? NetworkAddress.GetAddressBytes()[0] >= 224 && NetworkAddress.GetAddressBytes()[0] <= 239 : NetworkAddress.IsIPv6Multicast );
    public bool IsLinkLocal => NetworkAddress != null && ( AddressFamily == AddressFamily.InterNetwork ? NetworkAddress.GetAddressBytes()[0] == 169 && NetworkAddress.GetAddressBytes()[1] == 254 : NetworkAddress.IsIPv6LinkLocal );

    public IpNetwork ( IPAddress networkAddress , int prefixLength )
    {
        NetworkAddress = networkAddress ?? throw new ArgumentNullException ( nameof ( networkAddress ) );
        var maxPrefixLength = networkAddress.AddressFamily == AddressFamily.InterNetwork ? 32 : 128;

        if ( prefixLength < 0 || prefixLength > maxPrefixLength ) { throw new ArgumentException ( $"Ungültige Präfixlänge. Für {networkAddress.AddressFamily} muss sie zwischen 0 und {maxPrefixLength} liegen." , nameof ( prefixLength ) ); }

        PrefixLength = prefixLength;
    }

    public static IpNetwork Parse ( string cidr )
    {
        if ( string.IsNullOrWhiteSpace ( cidr ) ) { throw new ArgumentException ( "CIDR-String darf nicht null oder leer sein." , nameof ( cidr ) ); }

        var parts = cidr.Split ( '/' );

        if ( parts.Length != 2 ) { throw new ArgumentException ( "Ungültiges CIDR-Format. Erwartet wird 'Adresse/Präfixlänge'." , nameof ( cidr ) ); }

        if ( !IPAddress.TryParse ( parts[0] , out var ipAddress ) ) { throw new ArgumentException ( "Ungültige IP-Adresse im CIDR-String." , nameof ( cidr ) ); }

        if ( !int.TryParse ( parts[1] , out var prefixLength ) ) { throw new ArgumentException ( "Ungültige Präfixlänge im CIDR-String." , nameof ( cidr ) ); }

        var networkAddress = GetNetworkAddress ( ipAddress , prefixLength );

        return new IpNetwork ( networkAddress , prefixLength );
    }

    public static bool TryParse ( string cidr , out IpNetwork network )
    {
        network = default;

        try {
            network = Parse ( cidr );

            return true;
        }
        catch { return false; }
    }

    public bool Contains ( IPAddress ipAddress )
    {
        if ( ipAddress == null || NetworkAddress == null ) { return false; }

        if ( ipAddress.AddressFamily != NetworkAddress.AddressFamily ) { return false; }

        var ipBytes = ipAddress.GetAddressBytes();
        var networkBytes = NetworkAddress.GetAddressBytes();
        var maskBytes = GetSubnetMaskBytes();

        for ( var i = 0 ; i < ipBytes.Length ; i++ ) {
            if ( ( ipBytes[i] & maskBytes[i] ) != ( networkBytes[i] & maskBytes[i] ) ) { return false; }
        }

        return true;
    }

    public bool Contains ( string ipAddress )
    {
        if ( !IPAddress.TryParse ( ipAddress , out var ip ) ) { return false; }

        return Contains ( ip );
    }

    public bool Contains ( IpNetwork other )
    {
        if ( NetworkAddress == null || other.NetworkAddress == null ) { return false; }

        if ( AddressFamily != other.AddressFamily ) { return false; }

        if ( other.PrefixLength < PrefixLength ) { return false; }

        return Contains ( other.NetworkAddress );
    }

    public bool Overlaps ( IpNetwork other )
    {
        if ( NetworkAddress == null || other.NetworkAddress == null ) { return false; }

        if ( AddressFamily != other.AddressFamily ) { return false; }

        return Contains ( other.NetworkAddress ) || other.Contains ( NetworkAddress );
    }

    private byte[] GetSubnetMaskBytes()
    {
        if ( NetworkAddress == null ) { return new byte[ 0 ]; }

        var addressLength = NetworkAddress.AddressFamily == AddressFamily.InterNetwork ? 4 : 16;
        var maskBytes = new byte[ addressLength ];
        var fullBytes = PrefixLength / 8;
        var remainingBits = PrefixLength % 8;

        for ( var i = 0 ; i < fullBytes && i < maskBytes.Length ; i++ ) { maskBytes[i] = 0xFF; }

        if ( remainingBits > 0 && fullBytes < maskBytes.Length ) { maskBytes[fullBytes] = (byte) ( 0xFF << 8 - remainingBits ); }

        return maskBytes;
    }

    private static IPAddress GetNetworkAddress ( IPAddress ipAddress , int prefixLength )
    {
        var ipBytes = ipAddress.GetAddressBytes();
        var addressLength = ipAddress.AddressFamily == AddressFamily.InterNetwork ? 4 : 16;
        var maskBytes = new byte[ addressLength ];
        var fullBytes = prefixLength / 8;
        var remainingBits = prefixLength % 8;

        for ( var i = 0 ; i < fullBytes && i < maskBytes.Length ; i++ ) { maskBytes[i] = 0xFF; }

        if ( remainingBits > 0 && fullBytes < maskBytes.Length ) { maskBytes[fullBytes] = (byte) ( 0xFF << 8 - remainingBits ); }

        for ( var i = 0 ; i < ipBytes.Length ; i++ ) { ipBytes[i] &= maskBytes[i]; }

        return new IPAddress ( ipBytes );
    }

    public long GetHostCount()
    {
        if ( NetworkAddress == null ) { return 0; }

        var hostBits = ( NetworkAddress.AddressFamily == AddressFamily.InterNetwork ? 32 : 128 ) - PrefixLength;

        if ( hostBits >= 63 ) { return long.MaxValue; }

        var hostCount = 1L << hostBits;

        if ( NetworkAddress.AddressFamily == AddressFamily.InterNetwork && PrefixLength < 31 ) { hostCount -= 2; }

        return Math.Max ( 0 , hostCount );
    }

    #region Equality and ToString
    public bool Equals ( IpNetwork other ) { return NetworkAddress?.Equals ( other.NetworkAddress ) == true && PrefixLength == other.PrefixLength; }
    public override bool Equals ( object obj ) { return obj is IpNetwork other && Equals ( other ); }
    public override int GetHashCode() { return HashCode.Combine ( NetworkAddress , PrefixLength ); }
    public static bool operator == ( IpNetwork left , IpNetwork right ) { return left.Equals ( right ); }
    public static bool operator != ( IpNetwork left , IpNetwork right ) { return !left.Equals ( right ); }
    public override string ToString() { return NetworkAddress != null ? $"{NetworkAddress}/{PrefixLength}" : "Invalid"; }
    #endregion
}