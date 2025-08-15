// See https://aka.ms/new-console-template for more information

using WireGuardTools.Classes.Generators;

Console.WriteLine ( "Hello, World!" );
var tunnel = WgKeyGenerator.GenerateTunnelKeys ( 5 );

foreach ( var wgTunnel in tunnel ) {
    Console.WriteLine ( "Client.PrivateKey: " + wgTunnel.Client.PrivateKey.KeyBase64 );
    Console.WriteLine ( "Client.PublicKey: " + wgTunnel.Client.PublicKey.KeyBase64 );
    Console.WriteLine ( "Server.PrivateKey: " + wgTunnel.Server.PrivateKey.KeyBase64 );
    Console.WriteLine ( "Server.PublicKey: " + wgTunnel.Server.PublicKey.KeyBase64 );
    Console.WriteLine ( "PresharedKey: " + wgTunnel.PresharedKey.KeyBase64 );
    Console.WriteLine();
}
