using WireGuardTools.Classes.Base;

namespace WireGuardTools.Classes.Builders;

public interface IWgPeerBuilder
{
    IWgPeerBuilder WithPresharedKey ( WgBaseKey presharedKey );
    IWgPeerBuilder WithEndpoint ( string endpoint );
    IWgPeerBuilder WithAllowedIp ( string cidrAddress );
    IWgPeerBuilder WithPersistentKeepalive ( int seconds );
}