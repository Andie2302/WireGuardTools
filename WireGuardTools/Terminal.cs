using Renci.SshNet;
using Renci.SshNet.Common;

namespace WireGuardTools;

/// <summary>
/// Stellt eine SSH-Verbindung zu einem Server her, verwaltet diese und stellt Methoden zur 
/// Befehlsausführung bereit. Implementiert IDisposable zur automatischen Ressourcenfreigabe.
/// </summary>




public sealed class Terminal : IDisposable
{
    private const string AlreadyConnectedError = "Die Verbindung ist bereits hergestellt.";
    private const string NotConnectedError = "SSH-Verbindung ist nicht hergestellt. Bitte zuerst Connect() aufrufen.";
    private const string ObjectDisposedError = "Das Terminal-Objekt wurde bereits disposed.";
    
    private SshClient? _sshClient;
    private bool _disposed = false;
    
    /// <summary>
    /// Stellt eine Verbindung zum SSH-Server mit Passwort-Authentifizierung her.
    /// </summary>
    public void Connect(string host, string username, string password, int port = 22)
    {
        var parameters = new ConnectionParameters(host, port, username);
        var authStrategy = new PasswordAuthenticationStrategy(password);
        ConnectInternal(parameters, authStrategy);
    }
    
    /// <summary>
    /// Stellt eine Verbindung zum SSH-Server mit Schlüssel-Authentifizierung her (bevorzugt).
    /// </summary>
    public void ConnectWithKey(string host, string username, string privateKeyPath, string passphrase)
    {
        var parameters = new ConnectionParameters(host, 22, username);
        var authStrategy = new KeyAuthenticationStrategy(privateKeyPath, passphrase);
        ConnectInternal(parameters, authStrategy);
    }
    
    /// <summary>
    /// Führt einen Befehl auf dem Server aus und gibt das Ergebnis zurück.
    /// </summary>
    public string ExecuteCommand(string command)
    {
        ValidateIsConnected();
        
        using var cmd = _sshClient!.CreateCommand(command);
        return ExecuteAndValidateCommand(cmd, command);
    }
    
    /// <summary>
    /// Schließt die SSH-Verbindung.
    /// </summary>
    public void Close()
    {
        if (_sshClient?.IsConnected == true)
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
    
    private void ConnectInternal(ConnectionParameters parameters, IAuthenticationStrategy authStrategy)
    {
        ValidateCanConnect();
        
        SshClient? client = null;
        try
        {
            client = authStrategy.CreateClient(parameters);
            client.Connect();
            _sshClient = client;
        }
        catch (SshAuthenticationException ex)
        {
            client?.Dispose();
            throw new Exception($"SSH-Authentifizierungsfehler{authStrategy.AuthenticationTypeSuffix}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            client?.Dispose();
            throw new Exception($"Fehler beim Verbindungsaufbau{authStrategy.AuthenticationTypeSuffix}: {ex.Message}", ex);
        }
    }
    
    private void ValidateCanConnect()
    {
        ThrowIfDisposed();
        EnsureNotConnected();
    }
    
    private void ValidateIsConnected()
    {
        ThrowIfDisposed();
        EnsureConnected();
    }
    
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(Terminal), ObjectDisposedError);
        }
    }
    
    private void EnsureNotConnected()
    {
        if (_sshClient?.IsConnected == true)
        {
            throw new InvalidOperationException(AlreadyConnectedError);
        }
    }
    
    private void EnsureConnected()
    {
        if (_sshClient?.IsConnected != true)
        {
            throw new InvalidOperationException(NotConnectedError);
        }
    }
    
    private string ExecuteAndValidateCommand(SshCommand cmd, string command)
    {
        var output = cmd.Execute();
        
        if (cmd.ExitStatus != 0)
        {
            throw new SshCommandException(
                $"Befehl '{command}' schlug fehl mit Exit Code {cmd.ExitStatus}. Fehler: {cmd.Error}",
                cmd.ExitStatus,
                cmd.Error
            );
        }
        
        return output;
    }
}