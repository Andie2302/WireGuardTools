using System.Net;

namespace WireGuardTools.Scraps;

public record IpNetwork
{
    public IPAddress NetworkAddress { get; }
    public IPAddress BroadcastAddress { get; }
    public SubnetMask SubnetMask { get; }
    public IPAddress BaseAddress { get; }

    public IpNetwork ( IPAddress ipAddress , SubnetMask subnetMask )
    {
        ArgumentNullException.ThrowIfNull ( ipAddress );
        SubnetMask = subnetMask;
        BaseAddress = ipAddress;
        NetworkAddress = CalculateNetworkAddress ( ipAddress , subnetMask );
        BroadcastAddress = CalculateBroadcastAddress ( NetworkAddress , subnetMask );
    }

    public IpNetwork ( string ipAddress , SubnetMask subnetMask ) : this ( IPAddress.Parse ( ipAddress ) , subnetMask ) { }

    public static IpNetwork Parse ( string cidrNotation )
    {
        if ( string.IsNullOrWhiteSpace ( cidrNotation ) ) { throw new ArgumentException ( "CIDR-Notation darf nicht leer sein." , nameof ( cidrNotation ) ); }

        var parts = cidrNotation.Split ( '/' );

        if ( parts.Length != 2 ) { throw new ArgumentException ( "Ungültige CIDR-Notation. Erwartet: IP/Präfix" , nameof ( cidrNotation ) ); }

        var ipAddress = IPAddress.Parse ( parts[0] );
        var prefixLength = int.Parse ( parts[1] );

        return new IpNetwork ( ipAddress , SubnetMask.FromPrefixLength ( prefixLength ) );
    }

    public bool Contains ( IPAddress ipAddress )
    {
        if ( ipAddress == null ) { return false; }

        var targetBytes = ipAddress.GetAddressBytes();
        var networkBytes = NetworkAddress.GetAddressBytes();
        var maskBytes = SubnetMask.MaskAddress.GetAddressBytes();

        for ( var i = 0 ; i < 4 ; i++ ) {
            if ( ( targetBytes[i] & maskBytes[i] ) != networkBytes[i] ) { return false; }
        }

        return true;
    }

    public bool Contains ( string ipAddress ) => Contains ( IPAddress.Parse ( ipAddress ) );
    public long AddressCount => 1L << 32 - SubnetMask.PrefixLength;
    public long UsableAddressCount => Math.Max ( 0 , AddressCount - 2 );
    public IPAddress FirstUsableAddress => SubnetMask.PrefixLength >= 31 ? NetworkAddress : GetNextAddress ( NetworkAddress );
    public IPAddress LastUsableAddress => SubnetMask.PrefixLength >= 31 ? BroadcastAddress : GetPreviousAddress ( BroadcastAddress );

    public IPAddress GetNextAddress ( IPAddress currentAddress )
    {
        if ( !Contains ( currentAddress ) ) { throw new ArgumentException ( "Die IP-Adresse liegt nicht in diesem Netzwerk." , nameof ( currentAddress ) ); }

        var bytes = currentAddress.GetAddressBytes();
        var currentValue = (uint) ( bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3] );

        if ( currentAddress.Equals ( BroadcastAddress ) ) { return NetworkAddress; }

        currentValue++;
        var newBytes = new byte[ 4 ];
        newBytes[0] = (byte) ( currentValue >> 24 );
        newBytes[1] = (byte) ( currentValue >> 16 );
        newBytes[2] = (byte) ( currentValue >> 8 );
        newBytes[3] = (byte) currentValue;
        var nextAddress = new IPAddress ( newBytes );

        return Contains ( nextAddress ) ? nextAddress : NetworkAddress;
    }

    public IPAddress GetPreviousAddress ( IPAddress currentAddress )
    {
        if ( !Contains ( currentAddress ) ) { throw new ArgumentException ( "Die IP-Adresse liegt nicht in diesem Netzwerk." , nameof ( currentAddress ) ); }

        var bytes = currentAddress.GetAddressBytes();
        var currentValue = (uint) ( bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3] );

        if ( currentAddress.Equals ( NetworkAddress ) ) { return BroadcastAddress; }

        currentValue--;
        var newBytes = new byte[ 4 ];
        newBytes[0] = (byte) ( currentValue >> 24 );
        newBytes[1] = (byte) ( currentValue >> 16 );
        newBytes[2] = (byte) ( currentValue >> 8 );
        newBytes[3] = (byte) currentValue;
        var prevAddress = new IPAddress ( newBytes );

        return Contains ( prevAddress ) ? prevAddress : BroadcastAddress;
    }

    public IEnumerable< IPAddress > GetAllAddresses()
    {
        var current = NetworkAddress;
        var addresses = new List< IPAddress >();

        do {
            addresses.Add ( current );
            current = GetNextAddress ( current );
        }
        while ( !current.Equals ( NetworkAddress ) );

        return addresses;
    }

    public IEnumerable< IPAddress > GetUsableAddresses()
    {
        if ( SubnetMask.PrefixLength >= 31 ) {
            yield return NetworkAddress;

            if ( SubnetMask.PrefixLength == 31 ) { yield return BroadcastAddress; }

            yield break;
        }

        var current = FirstUsableAddress;
        var last = LastUsableAddress;

        while ( true ) {
            yield return current;

            if ( current.Equals ( last ) ) { break; }

            current = GetNextAddress ( current );
        }
    }

    private static IPAddress CalculateNetworkAddress ( IPAddress ipAddress , SubnetMask subnetMask )
    {
        var ipBytes = ipAddress.GetAddressBytes();
        var maskBytes = subnetMask.MaskAddress.GetAddressBytes();
        var networkBytes = new byte[ 4 ];

        for ( var i = 0 ; i < 4 ; i++ ) { networkBytes[i] = (byte) ( ipBytes[i] & maskBytes[i] ); }

        return new IPAddress ( networkBytes );
    }

    private static IPAddress CalculateBroadcastAddress ( IPAddress networkAddress , SubnetMask subnetMask )
    {
        var networkBytes = networkAddress.GetAddressBytes();
        var maskBytes = subnetMask.MaskAddress.GetAddressBytes();
        var broadcastBytes = new byte[ 4 ];

        for ( var i = 0 ; i < 4 ; i++ ) { broadcastBytes[i] = (byte) ( networkBytes[i] | ~maskBytes[i] ); }

        return new IPAddress ( broadcastBytes );
    }

    public override string ToString() => $"{NetworkAddress}/{SubnetMask.PrefixLength}";
    public string ToDetailedString() => $"Netzwerk: {NetworkAddress}/{SubnetMask.PrefixLength}\n" + $"Maske: {SubnetMask}\n" + $"Broadcast: {BroadcastAddress}\n" + $"Adressen: {AddressCount:N0} (davon {UsableAddressCount:N0} nutzbar)\n" + $"Bereich: {FirstUsableAddress} - {LastUsableAddress}";
}