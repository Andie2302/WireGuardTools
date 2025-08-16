using System.Net;

namespace WireGuardTools.Scraps;

public readonly record struct SubnetMask
{
    public int PrefixLength { get; }
    public IPAddress MaskAddress { get; }

    private SubnetMask ( int prefixLength , IPAddress maskAddress )
    {
        PrefixLength = prefixLength;
        MaskAddress = maskAddress;
    }

    public static SubnetMask FromPrefixLength ( int prefixLength )
    {
        if ( prefixLength < 0 || prefixLength > 32 ) { throw new ArgumentOutOfRangeException ( nameof ( prefixLength ) , "Präfixlänge muss zwischen 0 und 32 liegen." ); }

        var maskAddress = CreateMaskFromPrefix ( prefixLength );

        return new SubnetMask ( prefixLength , maskAddress );
    }

    public static SubnetMask FromMaskString ( string maskString )
    {
        if ( !IPAddress.TryParse ( maskString , out var ipAddress ) ) { throw new ArgumentException ( "Ungültige IP-Adresse für Netzmaske." , nameof ( maskString ) ); }

        return FromIpAddress ( ipAddress );
    }

    public static SubnetMask FromIpAddress ( IPAddress maskAddress )
    {
        ArgumentNullException.ThrowIfNull ( maskAddress );

        if ( !IsValidSubnetMask ( maskAddress ) ) { throw new ArgumentException ( "Die angegebene IP-Adresse ist keine gültige Subnetzmaske." , nameof ( maskAddress ) ); }

        var prefixLength = CalculatePrefixLength ( maskAddress );

        return new SubnetMask ( prefixLength , maskAddress );
    }

    public static bool IsValidSubnetMask ( IPAddress address )
    {
        if ( address is not
            {
                AddressFamily: System.Net.Sockets.AddressFamily.InterNetwork
            } ) { return false; }

        var bytes = address.GetAddressBytes();
        var maskValue = (uint) ( bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3] );
        var inverted = ~maskValue;
        var plusOne = inverted + 1;

        return maskValue == 0 || ( plusOne & inverted ) == 0;
    }

    private static IPAddress CreateMaskFromPrefix ( int prefixLength )
    {
        if ( prefixLength == 0 ) { return IPAddress.Any; }

        var maskValue = prefixLength == 32 ? 0xFFFFFFFF : ~( ( 1u << 32 - prefixLength ) - 1 );
        var bytes = new byte[ 4 ];
        bytes[0] = (byte) ( maskValue >> 24 );
        bytes[1] = (byte) ( maskValue >> 16 );
        bytes[2] = (byte) ( maskValue >> 8 );
        bytes[3] = (byte) maskValue;

        return new IPAddress ( bytes );
    }

    private static int CalculatePrefixLength ( IPAddress maskAddress )
    {
        var bytes = maskAddress.GetAddressBytes();
        var maskValue = (uint) ( bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3] );

        if ( maskValue == 0 ) { return 0; }

        var prefixLength = 0;

        for ( var i = 31 ; i >= 0 ; i-- ) {
            if ( ( maskValue & 1u << i ) != 0 ) { prefixLength++; }
            else { break; }
        }

        return prefixLength;
    }

    public static implicit operator SubnetMask ( int prefixLength ) => FromPrefixLength ( prefixLength );
    public static implicit operator SubnetMask ( string maskString ) => FromMaskString ( maskString );
    public override string ToString() => MaskAddress.ToString();
    public string ToCidrString() => $"/{PrefixLength}";
}