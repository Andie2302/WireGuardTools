using System.Diagnostics;
using FluentAssertions;
using WireGuardTools;

namespace WgTest;

/// <summary>
/// Performance tests to ensure key generation is reasonably fast
/// </summary>
public class PerformanceTests
{
    private readonly Curve25519KeyPairGenerator _generator;

    public PerformanceTests()
    {
        _generator = new Curve25519KeyPairGenerator();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void KeyGeneration_Performance_ShouldMeetBenchmarks2(int keyCount)
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var keyPairs = new List<WgKeyPair>();

        try
        {
            // Act
            for (var i = 0; i < keyCount; i++) keyPairs.Add(_generator.GenerateKeyPair());

            stopwatch.Stop();

            // Assert
            var avgMs = stopwatch.ElapsedMilliseconds / (double)keyCount;

            // Performance expectations (generous to account for different hardware)
            switch (keyCount)
            {
                case 10:
                    avgMs.Should().BeLessThan(100, "Small batches should be very fast");
                    break;
                case 100:
                    avgMs.Should().BeLessThan(50, "Medium batches should average < 50ms per key");
                    break;
                case 1000:
                    avgMs.Should().BeLessThan(25, "Large batches should average < 25ms per key");
                    break;
            }

            Console.WriteLine($"Generated {keyCount} key pairs in {stopwatch.ElapsedMilliseconds}ms " +
                              $"(avg: {avgMs:F2}ms per key)");
        }
        finally
        {
            // Cleanup
            foreach (var keyPair in keyPairs) keyPair.Dispose();
        }
    }

    [Fact]
    public void KeyGeneration_MemoryUsage_ShouldNotLeak2()
    {
        // This test ensures that key generation doesn't cause memory leaks
        const int iterations = 1000;

        // Force garbage collection before starting
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var initialMemory = GC.GetTotalMemory(false);

        // Generate and dispose many key pairs
        for (var i = 0; i < iterations; i++)
        {
            using var keyPair = _generator.GenerateKeyPair();
            // Access the keys to ensure they're fully initialized
            _ = keyPair.PrivateKey.Base64;
            _ = keyPair.PublicKey.Base64;
        }

        // Force garbage collection after
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var finalMemory = GC.GetTotalMemory(false);
        var memoryDifference = finalMemory - initialMemory;

        // Memory usage shouldn't increase significantly (allowing for some GC overhead)
        memoryDifference.Should().BeLessThan(1024 * 1024, // 1MB
            $"Memory usage increased by {memoryDifference} bytes, which suggests a memory leak");
    }
}