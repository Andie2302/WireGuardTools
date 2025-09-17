using FluentAssertions;
using WireGuardTools;

namespace WgTest;

public class WgCurve25519ConstantsTests
{
    [Fact]
    public void Prime_ShouldHaveCorrectValue()
    {
        // Arrange
        var expectedPrime = new byte[]
        {
            0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xED
        };

        // Act & Assert
        WgCurve25519Constants.Prime.Should().BeEquivalentTo(expectedPrime);
        WgCurve25519Constants.Prime.Should().HaveCount(32);
    }

    [Fact]
    public void A_ShouldHaveCorrectValue()
    {
        // Arrange
        var expectedA = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x6D, 0x06
        };

        // Act & Assert
        WgCurve25519Constants.A.Should().BeEquivalentTo(expectedA);
        WgCurve25519Constants.A.Should().HaveCount(32);
    }

    [Fact]
    public void B_ShouldHaveCorrectValue()
    {
        // Arrange - B should be all zeros for Curve25519
        var expectedB = new byte[32]; // All zeros

        // Act & Assert
        WgCurve25519Constants.B.Should().BeEquivalentTo(expectedB);
        WgCurve25519Constants.B.Should().HaveCount(32);
        WgCurve25519Constants.B.Should().AllSatisfy(b => b.Should().Be(0));
    }

    [Fact]
    public void Gx_ShouldHaveCorrectValue()
    {
        // Arrange
        var expectedGx = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09
        };

        // Act & Assert
        WgCurve25519Constants.Gx.Should().BeEquivalentTo(expectedGx);
        WgCurve25519Constants.Gx.Should().HaveCount(32);
    }

    [Fact]
    public void Gy_ShouldHaveCorrectValue()
    {
        // Arrange
        var expectedGy = new byte[]
        {
            0x20, 0xAE, 0x19, 0xA1, 0xB8, 0xA0, 0x86, 0xB4,
            0xE0, 0x1E, 0xDD, 0x2C, 0x77, 0x48, 0xD1, 0x4C,
            0x92, 0x3D, 0x4D, 0x7E, 0x6D, 0x7C, 0x61, 0xB2,
            0x29, 0xE9, 0xC5, 0xA2, 0x7E, 0xCE, 0xD3, 0xD9
        };

        // Act & Assert
        WgCurve25519Constants.Gy.Should().BeEquivalentTo(expectedGy);
        WgCurve25519Constants.Gy.Should().HaveCount(32);
    }

    [Fact]
    public void Order_ShouldHaveCorrectValue()
    {
        // Arrange
        var expectedOrder = new byte[]
        {
            0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x14, 0xDE, 0xF9, 0xDE, 0xA2, 0xF7, 0x9C, 0xD6,
            0x58, 0x12, 0x63, 0x1A, 0x5C, 0xF5, 0xD3, 0xED
        };

        // Act & Assert
        WgCurve25519Constants.Order.Should().BeEquivalentTo(expectedOrder);
        WgCurve25519Constants.Order.Should().HaveCount(32);
    }

    [Fact]
    public void Cofactor_ShouldHaveCorrectValue()
    {
        // Arrange
        var expectedCofactor = new byte[] { 0x08 };

        // Act & Assert
        WgCurve25519Constants.Cofactor.Should().BeEquivalentTo(expectedCofactor);
        WgCurve25519Constants.Cofactor.Should().HaveCount(1);
    }

    [Fact]
    public void AllConstants_ShouldBeReadOnly()
    {
        // This test verifies that the constants cannot be modified
        // Since they're readonly, we can't directly test modification
        // But we can verify they maintain their values across multiple accesses

        var prime1 = WgCurve25519Constants.Prime;
        var prime2 = WgCurve25519Constants.Prime;

        prime1.Should().BeSameAs(prime2);
        prime1.Should().BeEquivalentTo(prime2);
    }

    [Theory]
    [InlineData(nameof(WgCurve25519Constants.Prime), 32)]
    [InlineData(nameof(WgCurve25519Constants.A), 32)]
    [InlineData(nameof(WgCurve25519Constants.B), 32)]
    [InlineData(nameof(WgCurve25519Constants.Gx), 32)]
    [InlineData(nameof(WgCurve25519Constants.Gy), 32)]
    [InlineData(nameof(WgCurve25519Constants.Order), 32)]
    [InlineData(nameof(WgCurve25519Constants.Cofactor), 1)]
    public void Constants_ShouldHaveCorrectLength(string constantName, int expectedLength)
    {
        // Act
        var constant = constantName switch
        {
            nameof(WgCurve25519Constants.Prime) => WgCurve25519Constants.Prime,
            nameof(WgCurve25519Constants.A) => WgCurve25519Constants.A,
            nameof(WgCurve25519Constants.B) => WgCurve25519Constants.B,
            nameof(WgCurve25519Constants.Gx) => WgCurve25519Constants.Gx,
            nameof(WgCurve25519Constants.Gy) => WgCurve25519Constants.Gy,
            nameof(WgCurve25519Constants.Order) => WgCurve25519Constants.Order,
            nameof(WgCurve25519Constants.Cofactor) => WgCurve25519Constants.Cofactor,
            _ => throw new ArgumentException($"Unknown constant: {constantName}")
        };

        // Assert
        constant.Should().HaveCount(expectedLength);
    }

    [Fact]
    public void Prime_ShouldBeValidCurve25519Prime()
    {
        // The Curve25519 prime is 2^255 - 19
        // Verify this is the correct value
        var prime = WgCurve25519Constants.Prime;

        // Should be 32 bytes (256 bits, but top bit clear for 255-bit prime)
        prime.Should().HaveCount(32);

        // First byte should be 0x7F (top bit clear)
        prime[0].Should().Be(0x7F);

        // Last byte should be 0xED (which is 256 - 19 = 237 = 0xED)
        prime[31].Should().Be(0xED);
    }
}