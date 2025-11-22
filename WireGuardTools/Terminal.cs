using Renci.SshNet;
using Renci.SshNet.Common;

namespace WireGuardTools;

/// <summary>
/// Stellt eine SSH-Verbindung zu einem Server her, verwaltet diese und stellt Methoden zur 
/// Befehlsausführung bereit. Implementiert IDisposable zur automatischen Ressourcenfreigabe.
/// </summary>
public class Terminal : IDisposable
{
    private SshClient? _sshClient;
    private bool _disposed = false;

    /// <summary>
    /// Stellt eine Verbindung zum SSH-Server mit Passwort-Authentifizierung her.
    /// </summary>
    public void Connect(string host, string username, string password)
    {
        if (_sshClient != null && _sshClient.IsConnected)
        {
            throw new InvalidOperationException("Die Verbindung ist bereits hergestellt.");
        }
        
        try
        {
            _sshClient = new SshClient(host, username, password);
            _sshClient.Connect();
        }
        catch (SshAuthenticationException ex)
        {
            // Spezifische Ausnahme für Authentifizierungsfehler
            throw new Exception($"SSH-Authentifizierungsfehler: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // Andere Verbindungsfehler (Netzwerk, Host nicht erreichbar etc.)
            throw new Exception($"Fehler beim Verbindungsaufbau: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Stellt eine Verbindung zum SSH-Server mit Schlüssel-Authentifizierung her (bevorzugt).
    /// </summary>
    public void ConnectWithKey(string host, string username, string privateKeyPath, string passphrase)
    {
        if (_sshClient != null && _sshClient.IsConnected)
        {
            throw new InvalidOperationException("Die Verbindung ist bereits hergestellt.");
        }

        try
        {
            var privateKeyFile = new PrivateKeyFile(privateKeyPath, passphrase);
            var connectionInfo = new ConnectionInfo(
                host,
                username,
                new PrivateKeyAuthenticationMethod(username, privateKeyFile)
            );

            _sshClient = new SshClient(connectionInfo);
            _sshClient.Connect();
        }
        catch (SshAuthenticationException ex)
        {
            throw new Exception($"SSH-Authentifizierungsfehler (Key): {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Verbindungsaufbau (Key): {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Führt einen Befehl auf dem Server aus und gibt das Ergebnis zurück.
    /// </summary>
    public string ExecuteCommand(string command)
    {
        if (_sshClient == null || !_sshClient.IsConnected)
        {
            throw new InvalidOperationException("SSH-Verbindung ist nicht hergestellt. Bitte zuerst Connect() aufrufen.");
        }

        using (var cmd = _sshClient.CreateCommand(command))
        {
            // Die Execute()-Methode führt den Befehl aus und wartet auf die vollständige Beendigung.
            string output = cmd.Execute();

            if (cmd.ExitStatus != 0)
            {
                // Fehlerbehandlung: Bei einem Exit-Code ungleich Null einen Fehler werfen,
                // der die Fehlermeldung (Stderr) des Servers enthält.
                throw new SshCommandException(
                    $"Befehl '{command}' schlug fehl mit Exit Code {cmd.ExitStatus}. Fehler: {cmd.Error}", 
                    cmd.ExitStatus, 
                    cmd.Error
                );
            }

            return output;
        }
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
        // Sicherstellen, dass die Dispose-Methode nur einmal aufgerufen wird.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Freigabe verwalteter Ressourcen (SshClient)
                Close();
                _sshClient?.Dispose();
                _sshClient = null;
            }

            // Freigabe unmanaged Ressourcen (hier nicht relevant)

            _disposed = true;
        }
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