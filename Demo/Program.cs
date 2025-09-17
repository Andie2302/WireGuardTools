// See https://aka.ms/new-console-template for more information

using WgTest;
using WireGuardTools;

Console.WriteLine("Hello, World!");

Console.WriteLine("This is a test.");

Curve25519KeyPairGenerator generator = new();

for (var i = 0; i < 100; i++)
{
    using var keyPair = generator.GenerateKeyPair();
    Console.WriteLine(keyPair.PrivateKey.Base64);
    Console.WriteLine(keyPair.PublicKey.Base64);
    Console.WriteLine();
}
