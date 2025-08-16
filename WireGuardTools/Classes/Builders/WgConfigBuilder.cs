using System.Net;
using System.Net.Sockets;
using System.Text;
using WireGuardTools.Classes.Base;

namespace WireGuardTools.Classes.Builders;

public class IpNetwork
{
    private AddressFamily _addressFamily;
    public static IpNetwork Parse ( string cidrAddress ) => throw new NotImplementedException();
    public bool Contains ( IPAddress ip )
    {
        throw new NotImplementedException();
    }
    public bool Contains ( string text )
    {
        throw new NotImplementedException();
    }

    public long GetHostCount()
    {
        throw new NotImplementedException();
    }

    public AddressFamily AddressFamily
    {
        get { return _addressFamily; }
        set { _addressFamily = value; }
    }
}

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
        _peerConfig.AppendLine ( $"AllowedIPs = {IpNetwork.Parse ( cidrAddress )}" );
        return this;
    }

    public IWgPeerBuilder WithPersistentKeepalive ( int seconds )
    {
        _peerConfig.AppendLine ( $"PersistentKeepalive = {seconds}" );
        return this;
    }

    internal string Build() => _peerConfig.ToString();
}

public interface IWgConfigBuilder
{
    IWgConfigBuilder WithPrivateKey ( WgBaseKey privateKey );
    IWgConfigBuilder WithAddress ( string cidrAddress );
    IWgConfigBuilder WithListenPort ( int port );
    IWgConfigBuilder WithDns ( string dnsServer );
    IWgConfigBuilder AddPeer ( WgBaseKey publicKey , Action< IWgPeerBuilder > peerConfig );
    string Build();
}

public interface IWgPeerBuilder
{
    IWgPeerBuilder WithPresharedKey ( WgBaseKey presharedKey );
    IWgPeerBuilder WithEndpoint ( string endpoint );
    IWgPeerBuilder WithAllowedIp ( string cidrAddress );
    IWgPeerBuilder WithPersistentKeepalive ( int seconds );
}
