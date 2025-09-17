using FluentAssertions;
using WireGuardTools;

namespace WgTest;

public class WgKeyTests
{
    [Fact]
    public void Constructor_WithIncorrectBase64Length_ShouldGenerateRandomKey()
    {
        // Arrange - this Base64 string decodes to 27 bytes, not 32
        var incorrectBase64 = "////////////////////////////////////";
        var incorrectBytes = Convert.FromBase64String(incorrectBase64);
        incorrectBytes.Should().HaveCount(27, "This demonstrates the original test bug");

        // Act
        using var key = new WgKey(incorrectBytes);

        // Assert
        key.IsValid.Should().BeTrue();
        key.Size.Should().Be(32);
        // Should NOT be the incorrect bytes, but a new random key
        key.Key.Should().NotBeEquivalentTo(incorrectBytes);
        key.Key.Should().HaveCount(32);
    }

    [Fact]
    public void Constructor_WithoutParameters_ShouldCreateValidRandomKey()
    {
        // Act
        using var key = new WgKey();

        // Assert
        key.IsValid.Should().BeTrue();
        key.Size.Should().Be(WgTools.KeySize);
        key.Key.Length.Should().Be(32);
        key.Base64.Should().NotBeNullOrEmpty();
        Convert.FromBase64String(key.Base64).Length.Should().Be(32);
    }

    [Fact]
    public void Constructor_WithValidByteArray_ShouldUseProvidedKey()
    {
        // Arrange
        var testKey = new byte[32];
        for (int i = 0; i < 32; i++)
            testKey[i] = (byte)i;

        // Act
        using var key = new WgKey(testKey);

        // Assert
        key.IsValid.Should().BeTrue();
        key.Key.Should().BeEquivalentTo(testKey);
        key.Size.Should().Be(32);
    }

    [Fact]
    public void Constructor_WithInvalidByteArray_ShouldGenerateRandomKey()
    {
        // Arrange
        var invalidKey = new byte[16]; // Wrong size

        // Act
        using var key = new WgKey(invalidKey);

        // Assert
        key.IsValid.Should().BeTrue();
        key.Size.Should().Be(32);
        key.Key.Length.Should().Be(32);
        // The key should be different from the invalid input (different length anyway)
        key.Key.Should().HaveCount(32);
    }

    [Fact]
    public void Constructor_WithNullByteArray_ShouldGenerateRandomKey()
    {
        // Act
        using var key = new WgKey(null);

        // Assert
        key.IsValid.Should().BeTrue();
        key.Size.Should().Be(32);
        key.Base64.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CreateRandom_ShouldCreateValidRandomKey()
    {
        // Act
        using var key = WgKey.CreateRandom();

        // Assert
        key.IsValid.Should().BeTrue();
        key.Size.Should().Be(32);
    }

    [Fact]
    public void CreateRandom_MultipleCalls_ShouldCreateDifferentKeys()
    {
        // Act
        using var key1 = WgKey.CreateRandom();
        using var key2 = WgKey.CreateRandom();

        // Assert
        key1.Base64.Should().NotBe(key2.Base64);
        key1.Key.Should().NotBeEquivalentTo(key2.Key);
    }

    [Fact]
    public void Key_ShouldReturnCopyOfInternalArray()
    {
        // Arrange
        using var key = new WgKey();
        var firstCopy = key.Key;

        // Act
        var secondCopy = key.Key;
        firstCopy[0] = 0xFF; // Modify first copy

        // Assert
        secondCopy[0].Should().NotBe(0xFF);
        key.Key[0].Should().NotBe(0xFF);
    }

    [Fact]
    public void Base64_ShouldReturnValidBase64String()
    {
        // Arrange
        var testBytes = new byte[32];
        for (int i = 0; i < 32; i++)
            testBytes[i] = (byte)i;

        using var key = new WgKey(testBytes);

        // Act
        var base64 = key.Base64;

        // Assert
        base64.Should().NotBeNullOrEmpty();
        Convert.FromBase64String(base64).Should().BeEquivalentTo(testBytes);
    }

    [Fact]
    public void ToString_ShouldReturnBase64String()
    {
        // Arrange
        using var key = new WgKey();

        // Act
        var toString = key.ToString();

        // Assert
        toString.Should().Be(key.Base64);
    }

    [Fact]
    public void Regenerate_ShouldChangeKeyValue()
    {
        // Arrange
        using var key = new WgKey();
        var originalBase64 = key.Base64;

        // Act
        key.Regenerate();

        // Assert
        key.Base64.Should().NotBe(originalBase64);
        key.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Dispose_ShouldClearKeyAndMarkAsInvalid()
    {
        // Arrange
        var key = new WgKey();
        var keyData = key.Key;

        // Act
        key.Dispose();

        // Assert
        key.IsValid.Should().BeFalse();
    }

    [Fact]
    public void AccessAfterDispose_ShouldThrowObjectDisposedException()
    {
        // Arrange
        var key = new WgKey();
        key.Dispose();

        // Act & Assert
        key.Invoking(k => k.Key).Should().Throw<ObjectDisposedException>();
        key.Invoking(k => k.Base64).Should().Throw<ObjectDisposedException>();
        key.Invoking(k => k.Size).Should().Throw<ObjectDisposedException>();
        key.Invoking(k => k.Regenerate()).Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void MultipleDispose_ShouldNotThrow()
    {
        // Arrange
        var key = new WgKey();

        // Act & Assert
        key.Invoking(k => k.Dispose()).Should().NotThrow();
        key.Invoking(k => k.Dispose()).Should().NotThrow();
    }

    [Theory]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")] // 32 zero bytes
    [InlineData("MTIzNDU2Nzg5MDEyMzQ1Njc4OTAxMjM0NTY3ODkwMTI=")] // "123456789012345678901234567890123"
    [InlineData("//////////////////////////////////////////8=")] // 32 bytes of 0xFF
    public void Constructor_WithKnownBase64Values_ShouldWorkCorrectly(string base64)
    {
        // Arrange
        var expectedBytes = Convert.FromBase64String(base64);
        expectedBytes.Should().HaveCount(32, $"Test data should be 32 bytes, but {base64} decodes to {expectedBytes.Length} bytes");

        // Act
        using var key = new WgKey(expectedBytes);

        // Assert
        key.Base64.Should().Be(base64);
        key.Key.Should().BeEquivalentTo(expectedBytes);
    }

    [Fact]
    public void Constructor_WithAllOnesBytes_ShouldHandleEdgeCase()
    {
        // Arrange - test with all 0xFF bytes
        var allOnesBytes = Enumerable.Repeat((byte)0xFF, 32).ToArray();
        var expectedBase64 = "//////////////////////////////////////////8="; // Correct 32-byte representation

        // Act
        using var key = new WgKey(allOnesBytes);

        // Assert
        key.Key.Should().BeEquivalentTo(allOnesBytes);
        key.IsValid.Should().BeTrue();
        key.Base64.Should().Be(expectedBase64);

        // And that we can round-trip through Base64
        var roundTripBytes = Convert.FromBase64String(key.Base64);
        roundTripBytes.Should().BeEquivalentTo(allOnesBytes);
    }

    [Fact]
    public void KeyGeneration_ShouldProduceValidWireGuardKeys()
    {
        // This test ensures that generated keys follow WireGuard conventions
        // WireGuard keys are 32-byte values that are base64-encoded

        using var key = new WgKey();

        // Base64 encoded 32 bytes should be 44 characters (with padding)
        key.Base64.Length.Should().Be(44);
        key.Base64.Should().EndWith("=");

        // Should be valid base64
        var decoded = Convert.FromBase64String(key.Base64);
        decoded.Length.Should().Be(32);
    }
}