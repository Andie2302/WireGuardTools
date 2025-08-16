

using WireGuardTools.Classes.Generators;

Console.WriteLine ( "Hello, World!" );

var random = new SystemRandomGenerator();
var generator = new PreSharedKeyGenerator();
var wgKeyGenerator = new WgKeyGenerat

var tunnel = wgKeyGenerator.GenerateTunnelKeys ( 5 );

var clients = WgConfigGenerator.GenerateClientConfigs ( tunnel,"10.0.0.1","1.1.1.1" );
var server = WgConfigGenerator.GenerateServerConfig ( tunnel,"10.0.0.1",51820 );

Console.WriteLine ( server );
Console.WriteLine();
foreach ( var (_ , value) in clients ) {
    Console.WriteLine ( value );
    Console.WriteLine();
}
