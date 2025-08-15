using WireGuardTools.Classes.Base;
using WireGuardTools.Classes.Generators.Keys;

namespace WireGuardTools.Classes.Generators;

public class WgKeyGenerator ( IWgKeyGenerator wgKeyGenerator )
{
    private WgTunnelKeys GenerateTunnelKey ( WgKeys serverKeys ) => new WgTunnelKeys ( serverKeys , wgKeyGenerator.GenerateKeyPair() , wgKeyGenerator.GeneratePresharedKey() );

    public List< WgTunnelKeys > GenerateTunnelKeys ( int count )
    {
        var tunnels = new List< WgTunnelKeys >();
        var serverKey = wgKeyGenerator.GenerateKeyPair();

        for ( var i = 0 ; i < count ; i++ ) { tunnels.Add ( GenerateTunnelKey ( serverKey ) ); }

        return tunnels;
    }
}
