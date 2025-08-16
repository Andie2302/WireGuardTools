
using WireGuardTools.Classes;
using WireGuardTools.Classes.Engines;
using WireGuardTools.Classes.Random;

Console.WriteLine ( "Hello, World!" );


var wgKeyGenerator = new WgKeyGenerator ( new NsecKeyEngine(new SystemRandomGenerator()) );

var tunnel = wgKeyGenerator.GenerateTunnelKeys ( 5 );

foreach ( var wgTunnelKeys in tunnel ) {
    Console.WriteLine ( wgTunnelKeys.PresharedKey.KeyBase64 );
    Console.WriteLine();
}
/*
var clients = WgConfigGenerator.GenerateClientConfigs ( tunnel,"10.0.0.1","1.1.1.1" );
var server = WgConfigGenerator.GenerateServerConfig ( tunnel,"10.0.0.1",51820 );

Console.WriteLine ( server );
Console.WriteLine();
foreach ( var (_ , value) in clients ) {
    Console.WriteLine ( value );
    Console.WriteLine();
}
//*/