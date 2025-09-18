// See https://aka.ms/new-console-template for more information

using WgTest;
using WireGuardTools;

Console.WriteLine("Hello, World!");

Console.WriteLine("This is a test.");

Curve25519KeyPairGenerator generator = new();

foreach (var wgTunnel in WgTunnel.CreateRandom(10)) Console.WriteLine(wgTunnel);