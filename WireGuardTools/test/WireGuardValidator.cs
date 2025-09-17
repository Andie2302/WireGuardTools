using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WireGuardTools.test;

/// <summary>
/// Validates WireGuard key pairs against the official WireGuard tools
/// </summary>
public static class WireGuardValidator
{
    /// <summary>
    /// Validates a WireGuard key pair by checking if the public key matches what 'wg pubkey' produces
    /// </summary>
    /// <param name="keyPair">The key pair to validate</param>
    /// <returns>True if the key pair is valid and matches WireGuard tool output</returns>
    /// <exception cref="WireGuardToolException">Thrown when WireGuard tools are not available or return an error</exception>
    public static async Task<bool> ValidateKeyPairAsync(WgKeyPair keyPair)
    {
        if (!IsWireGuardToolAvailable())
            throw new WireGuardToolException("WireGuard tools are not available. Please install WireGuard.");

        try
        {
            var publicKeyFromTool = await GetPublicKeyFromWgToolAsync(keyPair.PrivateKey.Base64);
            return string.Equals(keyPair.PublicKey.Base64, publicKeyFromTool, StringComparison.Ordinal);
        }
        catch (Exception ex) when (!(ex is WireGuardToolException))
        {
            throw new WireGuardToolException($"Failed to validate key pair: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Validates a WireGuard key pair synchronously
    /// </summary>
    /// <param name="keyPair">The key pair to validate</param>
    /// <returns>True if the key pair is valid and matches WireGuard tool output</returns>
    public static bool ValidateKeyPair(WgKeyPair keyPair)
    {
        return ValidateKeyPairAsync(keyPair).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Generates a public key from a private key using the WireGuard tool
    /// </summary>
    /// <param name="privateKeyBase64">The private key in Base64 format</param>
    /// <returns>The corresponding public key in Base64 format</returns>
    /// <exception cref="WireGuardToolException">Thrown when WireGuard tools are not available or return an error</exception>
    public static async Task<string> GetPublicKeyFromWgToolAsync(string privateKeyBase64)
    {
        if (!IsWireGuardToolAvailable())
            throw new WireGuardToolException("WireGuard tools are not available. Please install WireGuard.");

        var wgCommand = GetWgCommand();
        var processStartInfo = new ProcessStartInfo
        {
            FileName = wgCommand,
            Arguments = "pubkey",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = Process.Start(processStartInfo);
        if (process == null) throw new WireGuardToolException("Could not start WireGuard process");

        try
        {
            // Write a private key to stdin
            await process.StandardInput.WriteLineAsync(privateKeyBase64);
            await process.StandardInput.FlushAsync();
            process.StandardInput.Close();

            // Read outputs
            var publicKeyTask = process.StandardOutput.ReadLineAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            // Wait for process completion with timeout
            var processExitTask = process.WaitForExitAsync();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

            if (await Task.WhenAny(processExitTask, timeoutTask) == timeoutTask)
            {
                process.Kill(true);
                throw new WireGuardToolException("WireGuard tool timed out after 10 seconds");
            }

            var publicKey = await publicKeyTask;
            var error = await errorTask;

            if (process.ExitCode != 0)
                throw new WireGuardToolException($"WireGuard tool failed with exit code {process.ExitCode}: {error}");

            return string.IsNullOrWhiteSpace(publicKey)
                ? throw new WireGuardToolException("WireGuard tool returned no output")
                : publicKey.Trim();
        }
        catch (Exception ex) when (!(ex is WireGuardToolException))
        {
            throw new WireGuardToolException($"Error executing WireGuard tool: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Checks if the WireGuard tool is available on the system
    /// </summary>
    /// <returns>True if the WireGuard tool is available</returns>
    public static bool IsWireGuardToolAvailable()
    {
        try
        {
            var wgCommand = GetWgCommand();
            var processStartInfo = new ProcessStartInfo
            {
                FileName = wgCommand,
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null) return false;

            var completed = process.WaitForExit(TimeSpan.FromSeconds(5));
            return completed && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the WireGuard tool version information
    /// </summary>
    /// <returns>Version string or null if not available</returns>
    public static async Task<string?> GetWireGuardVersionAsync()
    {
        if (!IsWireGuardToolAvailable()) return null;

        try
        {
            var wgCommand = GetWgCommand();
            var processStartInfo = new ProcessStartInfo
            {
                FileName = wgCommand,
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null) return null;

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return process.ExitCode == 0 ? output.Trim() : null;
        }
        catch
        {
            return null;
        }
    }

    private static string GetWgCommand()
    {
        // On Windows, it might be 'wg.exe', on Linux/Mac just 'wg'
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "wg.exe" : "wg";
    }
}