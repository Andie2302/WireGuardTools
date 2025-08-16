using WireGuardTools.Classes.Base;

namespace WireGuardTools.Classes.Builders;

public interface IWgConfigBuilder
{
    IWgConfigBuilder WithPrivateKey ( WgBaseKey privateKey );
    IWgConfigBuilder WithAddress ( string cidrAddress );
    IWgConfigBuilder WithListenPort ( int port );
    IWgConfigBuilder WithDns ( string dnsServer );
    IWgConfigBuilder AddPeer ( WgBaseKey publicKey , Action< IWgPeerBuilder > peerConfig );
    string Build();
}