using System.Text;
using WireGuardTools.Classes.Base;

namespace WireGuardTools.Classes.Builders;

internal class WgPeerBuilder : IWgPeerBuilder
{
    private readonly StringBuilder _peerConfig = new StringBuilder();

    public WgPeerBuilder ( WgBaseKey publicKey )
    {
        _peerConfig.AppendLine ( "[Peer]" );
        _peerConfig.AppendLine ( $"PublicKey = {publicKey.KeyBase64}" );
    }

    public IWgPeerBuilder WithPresharedKey ( WgBaseKey presharedKey )
    {
        _peerConfig.AppendLine ( $"PresharedKey = {presharedKey.KeyBase64}" );
        return this;
    }

    public IWgPeerBuilder WithEndpoint ( string endpoint )
    {
        _peerConfig.AppendLine ( $"Endpoint = {endpoint}" );
        return this;
    }

    public IWgPeerBuilder WithAllowedIp ( string cidrAddress )
    {
        //_peerConfig.AppendLine ( $"AllowedIPs = {IpNetwork.Parse ( cidrAddress )}" );
        return this;
    }

    public IWgPeerBuilder WithPersistentKeepalive ( int seconds )
    {
        _peerConfig.AppendLine ( $"PersistentKeepalive = {seconds}" );
        return this;
    }

    internal string Build() => _peerConfig.ToString();
}
