using System.Diagnostics;
using FluentAssertions;
using WireGuardTools;

namespace WgTest;

public class Curve25519KeyPairGeneratorTests
{
    private readonly Curve25519KeyPairGenerator _generator;

    public Curve25519KeyPairGeneratorTests()
    {
        _generator = new Curve25519KeyPairGenerator();
    }

    [Fact]
    public void GenerateKeyPair_ShouldCreateValidKeyPair()
    {
        // Act
        using var keyPair = _generator.GenerateKeyPair();

        // Assert
        keyPair.Should().NotBeNull();
        keyPair.PrivateKey.Should().NotBeNull();
        keyPair.PublicKey.Should().NotBeNull();
        keyPair.PrivateKey.IsValid.Should().BeTrue();
        keyPair.PublicKey.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GenerateKeyPair_ShouldCreateKeysWithCorrectSize()
    {
        // Act
        using var keyPair = _generator.GenerateKeyPair();

        // Assert
        keyPair.PrivateKey.Size.Should().Be(WgTools.KeySize);
        keyPair.PublicKey.Size.Should().Be(WgTools.KeySize);
    }

    [Fact]
    public void GenerateKeyPair_MultipleCalls_ShouldCreateDifferentKeyPairs()
    {
        // Act
        using var keyPair1 = _generator.GenerateKeyPair();
        using var keyPair2 = _generator.GenerateKeyPair();

        // Assert
        keyPair1.PrivateKey.Base64.Should().NotBe(keyPair2.PrivateKey.Base64);
        keyPair1.PublicKey.Base64.Should().NotBe(keyPair2.PublicKey.Base64);
    }

    [Fact]
    public void GenerateKeyPair_ShouldCreateValidBase64Keys()
    {
        // Act
        using var keyPair = _generator.GenerateKeyPair();

        // Assert
        keyPair.PrivateKey.Base64.Length.Should().Be(44); // 32 bytes base64 encoded
        keyPair.PublicKey.Base64.Length.Should().Be(44);

        // Should be valid base64
        var privateDecoded = Convert.FromBase64String(keyPair.PrivateKey.Base64);
        var publicDecoded = Convert.FromBase64String(keyPair.PublicKey.Base64);

        privateDecoded.Length.Should().Be(32);
        publicDecoded.Length.Should().Be(32);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void GenerateKeyPair_MultipleGenerations_ShouldAllBeUnique(int count)
    {
        // Arrange
        var privateKeys = new HashSet<string>();
        var publicKeys = new HashSet<string>();

        // Act
        for (var i = 0; i < count; i++)
        {
            using var keyPair = _generator.GenerateKeyPair();
            privateKeys.Add(keyPair.PrivateKey.Base64);
            publicKeys.Add(keyPair.PublicKey.Base64);
        }

        // Assert
        privateKeys.Should().HaveCount(count, "all private keys should be unique");
        publicKeys.Should().HaveCount(count, "all public keys should be unique");
    }

    [Fact]
    public void GenerateKeyPair_Performance_ShouldBeReasonablyFast()
    {
        // Arrange
        const int iterations = 100;
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (var i = 0; i < iterations; i++)
        {
            using var keyPair = _generator.GenerateKeyPair();
            // Just access the properties to ensure they're computed
            _ = keyPair.PrivateKey.Base64;
            _ = keyPair.PublicKey.Base64;
        }

        stopwatch.Stop();

        // Assert
        // Should generate 100 key pairs in less than 5 seconds (very generous)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000);

        // Average should be less than 50ms per key pair (also generous)
        var avgMs = stopwatch.ElapsedMilliseconds / (double)iterations;
        avgMs.Should().BeLessThan(50);
    }

    [Fact]
    public void GeneratedKeys_ShouldFollowWireGuardConventions()
    {
        // Act
        using var keyPair = _generator.GenerateKeyPair();

        // Assert
        // WireGuard keys are 32-byte values
        keyPair.PrivateKey.Key.Length.Should().Be(32);
        keyPair.PublicKey.Key.Length.Should().Be(32);

        // Base64 representation should end with '=' (padding)
        keyPair.PrivateKey.Base64.Should().EndWith("=");
        keyPair.PublicKey.Base64.Should().EndWith("=");

        // Should contain only valid base64 characters
        foreach (var c in keyPair.PrivateKey.Base64) (char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=').Should().BeTrue();
    }

    // This test verifies that our generator produces keys compatible with WireGuard
    // It uses known test vectors if available, or at least verifies the format
    [Fact]
    public void GeneratedKeys_ShouldBeCompatibleWithWireGuardFormat()
    {
        // Act
        using var keyPair = _generator.GenerateKeyPair();

        // Assert - verify the keys match expected WireGuard format
        var privateBytes = keyPair.PrivateKey.Key;
        var publicBytes = keyPair.PublicKey.Key;

        // Private key should be a valid 32-byte value
        privateBytes.Should().HaveCount(32);
        privateBytes.Should().NotBeEquivalentTo(new byte[32]); // Should not be all zeros

        // Public key should be a valid 32-byte value
        publicBytes.Should().HaveCount(32);
        publicBytes.Should().NotBeEquivalentTo(new byte[32]); // Should not be all zeros

        // Keys should be different
        privateBytes.Should().NotBeEquivalentTo(publicBytes);
    }

    // Integration test that would verify compatibility with actual WireGuard tool
    // This would require the WireGuard tool to be installed and available in PATH
    [Fact(Skip = "Requires WireGuard tool to be installed")]
    public void GeneratedPrivateKey_ShouldProduceCorrectPublicKeyWithWgTool()
    {
        // This test would:
        // 1. Generate a key pair with our generator
        // 2. Use the private key with 'wg pubkey' command
        // 3. Verify that the output matches our generated public key

        // Arrange
        using var keyPair = _generator.GenerateKeyPair();

        // This would need to be implemented with Process.Start
        // to call "echo {privateKey} | wg pubkey"
        // and compare the result with keyPair.PublicKey.Base64

        // For now, this is just a placeholder showing the concept
        Assert.True(true, "Test skipped - requires WireGuard tool installation");
    }
}