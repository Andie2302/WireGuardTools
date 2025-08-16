using WireGuardTools.Classes.Base;
using WireGuardTools.Interfaces;

namespace WireGuardTools.Classes;

public class WgKeyGenerator ( IWgKeyEngine wgKeyEngine )
{
    private WgTunnelKeys GenerateTunnelKey ( WgKeys serverKeys ) => new WgTunnelKeys ( serverKeys , wgKeyEngine.GenerateKeyPair() , wgKeyEngine.GeneratePresharedKey() );
    public List< WgTunnelKeys > GenerateTunnelKeys ( int count )
    {
        var tunnels = new List< WgTunnelKeys >();
        var serverKey = wgKeyEngine.GenerateKeyPair();

        for ( var i = 0 ; i < count ; i++ ) { tunnels.Add ( GenerateTunnelKey ( serverKey ) ); }

        return tunnels;
    }
}