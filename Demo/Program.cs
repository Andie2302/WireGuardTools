// See https://aka.ms/new-console-template for more information

using WireGuardTools;

Console.WriteLine("Hello, World!");

Console.WriteLine("This is a test.");

Curve25519KeyPairGenerator generator = new();
var wgKeyPair = generator.GenerateKeyPair();
var wgKeyPair2 = generator.GenerateKeyPair();
var wgKeyPair3 = generator.GenerateKeyPair();
var wgKeyPair4 = generator.GenerateKeyPair();
var wgKeyPair5 = generator.GenerateKeyPair();

Console.WriteLine(wgKeyPair.PrivateKey.ToString());
Console.WriteLine(wgKeyPair.PublicKey.ToString());
Console.WriteLine();
Console.WriteLine(wgKeyPair2.PrivateKey.ToString());
Console.WriteLine(wgKeyPair2.PublicKey.ToString());
Console.WriteLine();
Console.WriteLine(wgKeyPair3.PrivateKey.ToString());
Console.WriteLine(wgKeyPair3.PublicKey.ToString());
Console.WriteLine();
Console.WriteLine(wgKeyPair4.PrivateKey.ToString());
Console.WriteLine(wgKeyPair4.PublicKey.ToString());
Console.WriteLine();
Console.WriteLine(wgKeyPair5.PrivateKey.ToString());
Console.WriteLine(wgKeyPair5.PublicKey.ToString());

Console.WriteLine();

//hier soll eine Ausgabe für die konsole gemacht werden: echo key | wg pubkey
//für jeden privat key

Console.WriteLine($"echo {wgKeyPair.PrivateKey} | wg pubkey");
Console.WriteLine($"echo {wgKeyPair2.PrivateKey} | wg pubkey");
Console.WriteLine($"echo {wgKeyPair3.PrivateKey} | wg pubkey");
Console.WriteLine($"echo {wgKeyPair4.PrivateKey} | wg pubkey");
Console.WriteLine($"echo {wgKeyPair5.PrivateKey} | wg pubkey");
