using WireGuardTools;
using WireGuardTools.test;

namespace WgTest;

/// <summary>
/// Demo class showing how to use the WireGuard validator in practice
/// </summary>
public static class WireGuardValidatorDemo
{
    public static async Task RunDemo()
    {
        Console.WriteLine("🔧 WireGuard Validator Demo");
        Console.WriteLine(new string('*', 40));

        // Check if WireGuard tools are available
        var isAvailable = WireGuardValidator.IsWireGuardToolAvailable();
        Console.WriteLine($"WireGuard tools available: {(isAvailable ? "✅ Yes" : "❌ No")}");

        if (!isAvailable)
        {
            Console.WriteLine("Please install WireGuard tools to run this demo.");
            return;
        }

        // Get version
        var version = await WireGuardValidator.GetWireGuardVersionAsync();
        Console.WriteLine($"WireGuard version: {version}");
        Console.WriteLine();

        // Generate and validate key pairs
        var generator = new Curve25519KeyPairGenerator();

        Console.WriteLine("Generating and validating key pairs...");
        for (int i = 1; i <= 3; i++)
        {
            using var keyPair = generator.GenerateKeyPair();

            Console.WriteLine($"\nKey Pair {i}:");
            Console.WriteLine($"Private: {keyPair.PrivateKey.Base64}");
            Console.WriteLine($"Public:  {keyPair.PublicKey.Base64}");

            try
            {
                var isValid = await WireGuardValidator.ValidateKeyPairAsync(keyPair);
                Console.WriteLine($"Valid: {(isValid ? "✅ Yes" : "❌ No")}");

                if (isValid)
                {
                    var toolPublicKey = await WireGuardValidator.GetPublicKeyFromWgToolAsync(keyPair.PrivateKey.Base64);
                    Console.WriteLine($"Tool confirms: {toolPublicKey}");
                    Console.WriteLine($"Match: {keyPair.PublicKey.Base64 == toolPublicKey}");
                }
            }
            catch (WireGuardToolException ex)
            {
                Console.WriteLine($"❌ Validation failed: {ex.Message}");
            }
        }
    }
}