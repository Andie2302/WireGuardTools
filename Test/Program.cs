// See https://aka.ms/new-console-template for more information

using WireGuardTools.Generators;

Console.WriteLine("Hello, World!");

for ( var i = 0 ; i < 100 ; i++ )
{
    Console.WriteLine($"KeyPair #{i}");
    var pair = WgKeyPairGenerator.CreateNewWgKeyPair();
    Console.WriteLine(pair.PrivateKeyAsBase64);
    Console.WriteLine(pair.PublicKeyAsBase64);
    Console.WriteLine();
}