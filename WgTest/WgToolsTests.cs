using WireGuardTools;
using FluentAssertions;
using Xunit;

namespace WgTest;


public class WgToolsTests
{
    [Fact]
    public void KeySize_ShouldBe32Bytes()
    {
        // Act & Assert
        WgTools.KeySize.Should().Be(32);
    }

    [Fact]
    public void DefaultPort_ShouldBe51820()
    {
        // Act & Assert
        WgTools.DefaultPort.Should().Be(51820);
    }

    [Fact]
    public void Constants_ShouldBeWireGuardStandard()
    {
        // These are the standard WireGuard values as per RFC
        WgTools.KeySize.Should().Be(32, "WireGuard uses 32-byte (256-bit) keys");
        WgTools.DefaultPort.Should().Be(51820, "WireGuard standard port is 51820");
    }
}