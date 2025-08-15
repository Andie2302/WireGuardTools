// See https://aka.ms/new-console-template for more information

using WireGuardTools.ClassesOld.Generators;

Console.WriteLine ( "Hello, World!" );
var tunnel = WgKeyGenerator.GenerateTunnelKeys ( 5 );

var clients = WgConfigGenerator.GenerateClientConfigs ( tunnel,"10.0.0.1","1.1.1.1" );
var server = WgConfigGenerator.GenerateServerConfig ( tunnel,"10.0.0.1",51820 );

Console.WriteLine ( server );
Console.WriteLine();
foreach ( var (_ , value) in clients ) {
    Console.WriteLine ( value );
    Console.WriteLine();
}
