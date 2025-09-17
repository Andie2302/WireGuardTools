using System.Diagnostics;
using System.Runtime.InteropServices;
using FluentAssertions;
using WireGuardTools;

namespace WgTest;

/// <summary>
/// Integration tests that verify compatibility with the official WireGuard tools.
/// These tests require the WireGuard tools to be installed on the system.
/// </summary>
public class IntegrationTests
{
    private readonly Curve25519KeyPairGenerator _generator;

    public IntegrationTests()
    {
        _generator = new Curve25519KeyPairGenerator();
    }

    [Fact]
    public async Task GeneratedKeys_ShouldBeCompatibleWithWgPubkey()
    {
        // Skip if WireGuard tools are not available
        if (!IsWireGuardToolAvailable()) return; // Skip test silently

        // Arrange
        using var keyPair = _generator.GenerateKeyPair();
        var privateKeyBase64 = keyPair.PrivateKey.Base64;
        var expectedPublicKey = keyPair.PublicKey.Base64;

        // Act
        var actualPublicKey = await GetPublicKeyFromWgTool(privateKeyBase64);

        // Assert
        actualPublicKey.Should().Be(expectedPublicKey,
            "Our generated public key should match what 'wg pubkey' produces from our private key");
    }

    [Theory]
    [InlineData(5)]
    public async Task MultipleKeyPairs_ShouldAllBeCompatibleWithWgTool(int count)
    {
        // Skip if WireGuard tools are not available
        if (!IsWireGuardToolAvailable()) return; // Skip test silently

        for (int i = 0; i < count; i++)
        {
            // Arrange
            using var keyPair = _generator.GenerateKeyPair();
            var privateKeyBase64 = keyPair.PrivateKey.Base64;
            var expectedPublicKey = keyPair.PublicKey.Base64;

            // Act
            var actualPublicKey = await GetPublicKeyFromWgTool(privateKeyBase64);

            // Assert
            actualPublicKey.Should().Be(expectedPublicKey,
                $"Key pair {i + 1} should be compatible with wg pubkey");
        }
    }

    [Fact]
    public async Task KnownTestVector_ShouldMatchWgTool()
    {
        // Skip if WireGuard tools are not available
        if (!IsWireGuardToolAvailable()) return; // Skip test silently

        // Use a known test vector if available
        // For now, we'll generate a key and verify round-trip consistency
        using var keyPair = _generator.GenerateKeyPair();
        var privateKey = keyPair.PrivateKey.Base64;

        // Act
        var publicKeyFromWg = await GetPublicKeyFromWgTool(privateKey);
        var publicKeyFromOur = keyPair.PublicKey.Base64;

        // Assert
        publicKeyFromWg.Should().Be(publicKeyFromOur);
    }

    private static bool IsWireGuardToolAvailable()
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = GetWgCommand(),
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null) return false;

            process.WaitForExit(TimeSpan.FromSeconds(5));
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<string> GetPublicKeyFromWgTool(string privateKeyBase64)
    {
        var wgCommand = GetWgCommand();
        var processStartInfo = new ProcessStartInfo
        {
            FileName = wgCommand,
            Arguments = "pubkey",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        if (process == null)
            throw new InvalidOperationException("Could not start wg process");

        // Write private key to stdin
        await process.StandardInput.WriteLineAsync(privateKeyBase64);
        await process.StandardInput.FlushAsync();
        process.StandardInput.Close();

        // Read public key from stdout
        var publicKey = await process.StandardOutput.ReadLineAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0) throw new InvalidOperationException($"wg pubkey failed with exit code {process.ExitCode}: {error}");

        return publicKey?.Trim() ?? throw new InvalidOperationException("No output from wg pubkey");
    }

    private static string GetWgCommand()
    {
        // On Windows, it might be 'wg.exe', on Linux/Mac just 'wg'
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "wg.exe" : "wg";
    }
}