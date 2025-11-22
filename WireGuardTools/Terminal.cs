using Renci.SshNet;
using Renci.SshNet.Common;

namespace WireGuardTools;

/// <summary>
/// Stellt eine SSH-Verbindung zu einem Server her, verwaltet diese und stellt Methoden zur 
/// Befehlsausführung bereit. Implementiert IDisposable zur automatischen Ressourcenfreigabe.
/// </summary>

public sealed class Terminal : IDisposable
{
    private SshClient? _sshClient;
    private bool _disposed = false;

    /// <summary>
    /// Stellt eine Verbindung zum SSH-Server mit Passwort-Authentifizierung her.
    /// </summary>
    public void Connect(string host, string username, string password)
    {
        EnsureNotConnected();
        ConnectInternal(() => new SshClient(host, username, password), "");
    }

    /// <summary>
    /// Stellt eine Verbindung zum SSH-Server mit Schlüssel-Authentifizierung her (bevorzugt).
    /// </summary>
    public void ConnectWithKey(string host, string username, string privateKeyPath, string passphrase)
    {
        EnsureNotConnected();
        ConnectInternal(() => CreateSshClientWithKey(host, username, privateKeyPath, passphrase), " (Key)");
    }

    /// <summary>
    /// Führt einen Befehl auf dem Server aus und gibt das Ergebnis zurück.
    /// </summary>
    public string ExecuteCommand(string command)
    {
        EnsureConnected();

        using var cmd = _sshClient?.CreateCommand(command);
        var output = cmd?.Execute();
        if (cmd is not { ExitStatus: 0 })
        {
            throw new SshCommandException(
                $"Befehl '{command}' schlug fehl mit Exit Code {cmd?.ExitStatus}. Fehler: {cmd?.Error}", 
                cmd.ExitStatus, 
                cmd.Error
            );
        }
        return output ?? string.Empty;
    }

    /// <summary>
    /// Schließt die SSH-Verbindung.
    /// </summary>
    public void Close()
    {
        if (_sshClient != null && _sshClient.IsConnected)
        {
            _sshClient.Disconnect();
        }
    }

    // --- IDisposable Implementierung ---
    /// <summary>
    /// Implementiert IDisposable: Gibt die verwalteten Ressourcen frei.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            Close();
            _sshClient?.Dispose();
            _sshClient = null;
        }
        _disposed = true;
    }

    private void EnsureNotConnected()
    {
        if (_sshClient is { IsConnected: true })
        {
            throw new InvalidOperationException("Die Verbindung ist bereits hergestellt.");
        }
    }

    private void EnsureConnected()
    {
        if (_sshClient is not { IsConnected: true })
        {
            throw new InvalidOperationException("SSH-Verbindung ist nicht hergestellt. Bitte zuerst Connect() aufrufen.");
        }
    }

    private void ConnectInternal(Func<SshClient> createClient, string errorSuffix)
    {
        try
        {
            _sshClient = createClient();
            _sshClient.Connect();
        }
        catch (SshAuthenticationException ex)
        {
            throw new Exception($"SSH-Authentifizierungsfehler{errorSuffix}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Verbindungsaufbau{errorSuffix}: {ex.Message}", ex);
        }
    }

    private static SshClient CreateSshClientWithKey(string host, string username, string privateKeyPath, string passphrase)
    {
        var privateKeyFile = new PrivateKeyFile(privateKeyPath, passphrase);
        var connectionInfo = new ConnectionInfo(
            host,
            username,
            new PrivateKeyAuthenticationMethod(username, privateKeyFile)
        );
        return new SshClient(connectionInfo);
    }
}

// Optional: Eine benutzerdefinierte Ausnahme für die Befehlsausführung, 
// um den Exit Code und Error-Stream besser zu transportieren.
public class SshCommandException : Exception
{
    public int ExitCode { get; }
    public string ErrorOutput { get; }

    public SshCommandException(string message, int exitCode, string errorOutput) 
        : base(message)
    {
        ExitCode = exitCode;
        ErrorOutput = errorOutput;
    }
}