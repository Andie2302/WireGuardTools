using System.Text;
using WireGuardTools.ClassesOld.Base;

namespace WireGuardTools.ClassesOld.Generators;

public static class WgConfigGenerator
{
    public static string GenerateServerConfig ( List< WgTunnelKeys > tunnels , string serverAddress , int listenPort )
    {
        if ( tunnels.Count == 0 ) { return string.Empty; }

        var server = tunnels[0].Server;
        var configBuilder = new StringBuilder();
        configBuilder.AppendLine ( "# ===== Server Konfiguration ===== #" );
        configBuilder.AppendLine ( "[Interface]" );
        configBuilder.AppendLine ( $"Address = {serverAddress}" );
        configBuilder.AppendLine ( $"ListenPort = {listenPort}" );
        configBuilder.AppendLine ( $"PrivateKey = {server.PrivateKey.KeyBase64}" );
        configBuilder.AppendLine ( "#PostUp = iptables -A FORWARD -i %i -j ACCEPT; iptables -t nat -A POSTROUTING -o <netzwerk-interface> -j MASQUERADE" );
        configBuilder.AppendLine ( "#PostDown = iptables -D FORWARD -i %i -j ACCEPT; iptables -t nat -D POSTROUTING -o <netzwerk-interface> -j MASQUERADE" );
        configBuilder.AppendLine();

        for ( var i = 0 ; i < tunnels.Count ; i++ ) {
            var tunnel = tunnels[i];
            var clientIp = $"10.0.0.{i + 2}/32";
            configBuilder.AppendLine ( $"# ===== Client {i + 1} ===== #" );
            configBuilder.AppendLine ( "[Peer]" );
            configBuilder.AppendLine ( $"PublicKey = {tunnel.Client.PublicKey.KeyBase64}" );
            configBuilder.AppendLine ( $"PresharedKey = {tunnel.PresharedKey.KeyBase64}" );
            configBuilder.AppendLine ( $"AllowedIPs = {clientIp}" );
            configBuilder.AppendLine();
        }

        return configBuilder.ToString();
    }

    public static Dictionary< string , string > GenerateClientConfigs ( List< WgTunnelKeys > tunnels , string serverEndpoint , string dns )
    {
        if ( tunnels.Count == 0 ) {
            return new Dictionary< string , string >();
        }

        var serverPublicKey = tunnels[0].Server.PublicKey;
        var clientConfigs = new Dictionary< string , string >();

        for ( var i = 0 ; i < tunnels.Count ; i++ ) {
            var tunnel = tunnels[i];
            var clientAddress = $"10.0.0.{i + 2}/32";
            var configBuilder = new StringBuilder();
            configBuilder.AppendLine ( $"# ===== Client {i + 1} Konfiguration ===== #" );
            configBuilder.AppendLine ( "[Interface]" );
            configBuilder.AppendLine ( $"PrivateKey = {tunnel.Client.PrivateKey.KeyBase64}" );
            configBuilder.AppendLine ( $"Address = {clientAddress}" );
            configBuilder.AppendLine ( $"DNS = {dns}" );
            configBuilder.AppendLine();
            configBuilder.AppendLine ( "# ===== Server ===== #" );
            configBuilder.AppendLine ( "[Peer]" );
            configBuilder.AppendLine ( $"PublicKey = {serverPublicKey.KeyBase64}" );
            configBuilder.AppendLine ( $"PresharedKey = {tunnel.PresharedKey.KeyBase64}" );
            configBuilder.AppendLine ( $"Endpoint = {serverEndpoint}" );
            configBuilder.AppendLine ( "AllowedIPs = 0.0.0.0/0, ::/0" );
            clientConfigs.Add ( $"wg-client-{i + 1}.conf" , configBuilder.ToString() );
        }

        return clientConfigs;
    }
}