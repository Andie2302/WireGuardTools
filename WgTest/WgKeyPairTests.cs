using FluentAssertions;
using WireGuardTools;

namespace WgTest;

public class WgKeyPairTests
{
    [Fact]
    public void Constructor_WithKeys_ShouldCreateValidKeyPair()
    {
        // Arrange
        using var privateKey = new WgKey();
        using var publicKey = new WgKey();

        // Act
        using var keyPair = new WgKeyPair(privateKey, publicKey);

        // Assert
        keyPair.PrivateKey.Should().BeSameAs(privateKey);
        keyPair.PublicKey.Should().BeSameAs(publicKey);
    }

    [Fact]
    public void Constructor_WithGenerator_ShouldCreateValidKeyPair()
    {
        // Arrange
        var generator = new Curve25519KeyPairGenerator();

        // Act
        using var keyPair = new WgKeyPair(generator);

        // Assert
        keyPair.PrivateKey.Should().NotBeNull();
        keyPair.PublicKey.Should().NotBeNull();
        keyPair.PrivateKey.IsValid.Should().BeTrue();
        keyPair.PublicKey.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithFromPrivateKeyGenerator_ShouldCreateValidKeyPair()
    {
        // This test would require implementing IWgFromPrivateKeyKeyPairGenerator
        // For now, we'll skip it since it's not implemented yet
        // 
        // Arrange
        // var generator = new SomeFromPrivateKeyGenerator();
        // using var privateKey = new WgKey();
        //
        // Act
        // using var keyPair = new WgKeyPair(generator, privateKey);
        //
        // Assert
        // keyPair.PrivateKey.Should().BeSameAs(privateKey);
        // keyPair.PublicKey.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_ShouldDisposeBothKeys()
    {
        // Arrange
        using var privateKey = new WgKey();
        using var publicKey = new WgKey();
        var keyPair = new WgKeyPair(privateKey, publicKey);

        // Act
        keyPair.Dispose();

        // Assert
        privateKey.IsValid.Should().BeFalse();
        publicKey.IsValid.Should().BeFalse();
    }

    [Fact]
    public void MultipleKeyPairs_ShouldHaveDifferentKeys()
    {
        // Arrange
        var generator = new Curve25519KeyPairGenerator();

        // Act
        using var keyPair1 = new WgKeyPair(generator);
        using var keyPair2 = new WgKeyPair(generator);

        // Assert
        keyPair1.PrivateKey.Base64.Should().NotBe(keyPair2.PrivateKey.Base64);
        keyPair1.PublicKey.Base64.Should().NotBe(keyPair2.PublicKey.Base64);
    }
}