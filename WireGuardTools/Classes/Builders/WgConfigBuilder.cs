using System.Text;
using WireGuardTools.Classes.Base;

namespace WireGuardTools.Classes.Builders;

public class WgConfigBuilder : IWgConfigBuilder
{
    private WgBaseKey? _privateKey;
    private readonly List< IpNetwork > _addresses = [ ];
    private int? _listenPort;
    private readonly List< string > _dnsServers = [ ];
    private readonly StringBuilder _peers = new StringBuilder();

    public IWgConfigBuilder WithPrivateKey ( WgBaseKey privateKey )
    {
        _privateKey = privateKey;
        return this;
    }

    public IWgConfigBuilder WithAddress ( string cidrAddress )
    {
        _addresses.Add ( IpNetwork.Parse ( cidrAddress ) );
        return this;
    }

    public IWgConfigBuilder WithListenPort ( int port )
    {
        _listenPort = port;
        return this;
    }

    public IWgConfigBuilder WithDns ( string dnsServer )
    {
        _dnsServers.Add ( dnsServer );
        return this;
    }

    public IWgConfigBuilder AddPeer ( WgBaseKey publicKey , Action< IWgPeerBuilder > peerConfig )
    {
        var peerBuilder = new WgPeerBuilder ( publicKey );
        peerConfig ( peerBuilder );
        _peers.AppendLine ( peerBuilder.Build() );
        _peers.AppendLine();
        return this;
    }

    public string Build()
    {
        if ( _privateKey is null ) {
            throw new InvalidOperationException ( "Ein PrivateKey muss für das [Interface] gesetzt sein." );
        }

        if ( _addresses.Count == 0 ) {
            throw new InvalidOperationException ( "Mindestens eine Adresse muss für das [Interface] gesetzt sein." );
        }

        var builder = new StringBuilder();
        builder.AppendLine ( "[Interface]" );
        builder.AppendLine ( $"PrivateKey = {_privateKey.Value.KeyBase64}" );
        builder.AppendLine ( $"Address = {string.Join ( ", " , _addresses )}" );
        if ( _listenPort.HasValue ) {
            builder.AppendLine ( $"ListenPort = {_listenPort.Value}" );
        }

        if ( _dnsServers.Count != 0) {
            builder.AppendLine ( $"DNS = {string.Join ( ", " , _dnsServers )}" );
        }

        builder.AppendLine();
        builder.Append ( _peers);

        return builder.ToString().Trim();
    }
}