// See https://aka.ms/new-console-template for more information

using WireGuardTools.Generators;

// Generate multiple key pairs efficiently
foreach (var keyPair in WgPeerGenerator.Create(5))
{
    Console.WriteLine($"Private: {keyPair.PrivateKeyAsBase64}");
    Console.WriteLine($"Public:  {keyPair.PublicKeyAsBase64}");
    Console.WriteLine();
}