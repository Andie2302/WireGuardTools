using FluentAssertions;
using WireGuardTools;
using WireGuardTools.test;
using Xunit.Abstractions;

namespace WgTest;

public class WireGuardValidatorTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Curve25519KeyPairGenerator _generator;

    public WireGuardValidatorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _generator = new Curve25519KeyPairGenerator();
    }

    [Fact]
    public async Task ValidateKeyPairAsync_WithValidKeyPair_ShouldReturnTrue()
    {
        // Skip if WireGuard tools are not available
        if (!WireGuardValidator.IsWireGuardToolAvailable()) return; // Skip test

        // Arrange
        using var keyPair = _generator.GenerateKeyPair();

        // Act
        var isValid = await WireGuardValidator.ValidateKeyPairAsync(keyPair);

        // Assert
        isValid.Should().BeTrue("Generated key pair should be valid according to WireGuard tools");
    }

    [Fact]
    public void ValidateKeyPair_WithValidKeyPair_ShouldReturnTrue()
    {
        // Skip if WireGuard tools are not available
        if (!WireGuardValidator.IsWireGuardToolAvailable()) return; // Skip test

        // Arrange
        using var keyPair = _generator.GenerateKeyPair();

        // Act
        var isValid = WireGuardValidator.ValidateKeyPair(keyPair);

        // Assert
        isValid.Should().BeTrue("Generated key pair should be valid according to WireGuard tools");
    }

    [Fact]
    public async Task ValidateKeyPairAsync_WithMismatchedKeys_ShouldReturnFalse()
    {
        // Skip if WireGuard tools are not available
        if (!WireGuardValidator.IsWireGuardToolAvailable()) return; // Skip test

        // Arrange - create a key pair with mismatched keys
        using var privateKey = new WgKey();
        using var wrongPublicKey = new WgKey(); // This is not the correct public key
        using var mismatchedKeyPair = new WgKeyPair(privateKey, wrongPublicKey);

        // Act
        var isValid = await WireGuardValidator.ValidateKeyPairAsync(mismatchedKeyPair);

        // Assert
        isValid.Should().BeFalse("Mismatched key pair should not be valid");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    public async Task ValidateMultipleKeyPairs_ShouldAllBeValid(int count)
    {
        // Skip if WireGuard tools are not available
        if (!WireGuardValidator.IsWireGuardToolAvailable()) return; // Skip test

        var results = new List<bool>();

        // Act
        for (int i = 0; i < count; i++)
        {
            using var keyPair = _generator.GenerateKeyPair();
            var isValid = await WireGuardValidator.ValidateKeyPairAsync(keyPair);
            results.Add(isValid);
        }

        // Assert
        results.Should().AllSatisfy(result => result.Should().BeTrue($"All generated key pairs should be valid"));
    }

    [Fact]
    public async Task GetPublicKeyFromWgToolAsync_WithValidPrivateKey_ShouldReturnCorrectPublicKey()
    {
        // Skip if WireGuard tools are not available
        if (!WireGuardValidator.IsWireGuardToolAvailable()) return; // Skip test

        // Arrange
        using var keyPair = _generator.GenerateKeyPair();

        // Act
        var publicKeyFromTool = await WireGuardValidator.GetPublicKeyFromWgToolAsync(keyPair.PrivateKey.Base64);

        // Assert
        publicKeyFromTool.Should().Be(keyPair.PublicKey.Base64);
    }

    [Fact]
    public void IsWireGuardToolAvailable_ShouldReturnBooleanValue()
    {
        // Act
        var isAvailable = WireGuardValidator.IsWireGuardToolAvailable();

        // Assert - Diese Assertion ist eigentlich überflüssig,
        // da ein bool nur true oder false sein kann
        // Aber wenn Sie darauf bestehen:
        (isAvailable == true || isAvailable == false).Should().BeTrue();

        // Oder besser - testen Sie das Verhalten:
        var action = () => WireGuardValidator.IsWireGuardToolAvailable();
        action.Should().NotThrow();

        _testOutputHelper.WriteLine(isAvailable
            ? "✅ WireGuard tools are available on this system"
            : "⚠️  WireGuard tools are not available on this system");
    }

    [Fact]
    public void IsWireGuardToolAvailable_ShouldNotThrow()
    {
        // Act & Assert - Das ist ein viel besserer Test
        var action = () => WireGuardValidator.IsWireGuardToolAvailable();
        action.Should().NotThrow();

        var isAvailable = action();
        _testOutputHelper.WriteLine(isAvailable
            ? "✅ WireGuard tools are available on this system"
            : "⚠️  WireGuard tools are not available on this system");
    }

    [Fact]
    public async Task GetWireGuardVersionAsync_WhenAvailable_ShouldReturnVersionString()
    {
        // Skip if WireGuard tools are not available
        if (!WireGuardValidator.IsWireGuardToolAvailable()) return; // Skip test

        // Act
        var version = await WireGuardValidator.GetWireGuardVersionAsync();

        // Assert
        version.Should().NotBeNullOrWhiteSpace("Version should be returned when WireGuard is available");
        Console.WriteLine($"WireGuard version: {version}");
    }

    [Fact]
    public async Task ValidateKeyPairAsync_WithoutWireGuardTool_ShouldThrowException()
    {
        // This test would need to mock the WireGuard tool not being available
        // For now, we'll just test the exception type if tools are not available

        if (WireGuardValidator.IsWireGuardToolAvailable()) return; // Skip if tools are available

        // Arrange
        using var keyPair = _generator.GenerateKeyPair();

        // Act & Assert
        await keyPair.Invoking(async kp => await WireGuardValidator.ValidateKeyPairAsync(kp))
            .Should().ThrowAsync<WireGuardToolException>()
            .WithMessage("*WireGuard tools are not available*");
    }
}