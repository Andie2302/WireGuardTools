using Renci.SshNet;
using Renci.SshNet.Common;
using System.IO;

namespace WireGuardTools;

/// <summary>
/// Manages the lifecycle of the SSH and SCP connections to a remote host based on provided settings.
/// </summary>
public sealed class TerminalConnection : IDisposable
{
    private const string AlreadyConnectedError = "The connection is already established.";
    private const string NotConnectedError = "SSH connection is not established. Please call Connect() first.";
    private const string ObjectDisposedError = "The TerminalConnection object has already been disposed.";

    private readonly ConnectionSettings _settings;
    private SshClient? _sshClient;
    private ScpClient? _scpClient; // Wird für Datei-Operationen benötigt
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalConnection"/> class.
    /// </summary>
    /// <param name="settings">The connection settings to use for establishing the connection.</param>
    public TerminalConnection(ConnectionSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Establishes the SSH and SCP connections to the remote host.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if already connected or disposed.</exception>
    /// <exception cref="Exception">Thrown on authentication or connection failure.</exception>
    public void Connect()
    {
        ValidateCanConnect();

        ConnectionInfo connectionInfo;
        string authTypeSuffix;

        // Pattern Matching zur Erstellung der ConnectionInfo basierend auf den Einstellungen
        if (_settings is PasswordConnectionSettings passwordSettings)
        {
            connectionInfo = new ConnectionInfo(
                passwordSettings.Host, 
                passwordSettings.Port, 
                passwordSettings.Username, 
                new PasswordAuthenticationMethod(passwordSettings.Username, passwordSettings.Password)
            );
            authTypeSuffix = "";
        }
        else if (_settings is KeyConnectionSettings keySettings)
        {
            // Achtung: PrivateKeyFile muss disposed werden
            using var privateKeyFile = new PrivateKeyFile(keySettings.PrivateKeyPath, keySettings.Passphrase);
            connectionInfo = new ConnectionInfo(
                keySettings.Host,
                keySettings.Port,
                keySettings.Username,
                new PrivateKeyAuthenticationMethod(keySettings.Username, privateKeyFile)
            );
            authTypeSuffix = " (Key)";
        }
        else
        {
            throw new NotSupportedException("Unsupported connection settings type.");
        }

        SshClient? sshClient = null;
        ScpClient? scpClient = null;

        try
        {
            // SSH Client (für Befehle)
            sshClient = new SshClient(connectionInfo);
            sshClient.Connect();
            _sshClient = sshClient;

            // SCP Client (für Datei-Übertragung)
            scpClient = new ScpClient(connectionInfo);
            scpClient.Connect();
            _scpClient = scpClient;
        }
        catch (SshAuthenticationException ex)
        {
            sshClient?.Dispose();
            scpClient?.Dispose();
            throw new Exception($"SSH-Authentifizierungsfehler{authTypeSuffix}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            sshClient?.Dispose();
            scpClient?.Dispose();
            throw new Exception($"Fehler beim Verbindungsaufbau{authTypeSuffix}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Executes a given command on the remote server.
    /// </summary>
    public string ExecuteCommand(string command)
    {
        ValidateIsConnected();
        
        using var cmd = _sshClient!.CreateCommand(command);
        return ExecuteAndValidateCommand(cmd, command);
    }
    
    /// <summary>
    /// Downloads a file from the remote host.
    /// </summary>
    public void DownloadFile(string remotePath, string localPath)
    {
        ValidateIsConnected();
        _scpClient!.Download(remotePath, new FileInfo(localPath));
    }
    
    /// <summary>
    /// Uploads a file to the remote host.
    /// </summary>
    public void UploadFile(string localPath, string remotePath)
    {
        ValidateIsConnected();
        _scpClient!.Upload(new FileInfo(localPath), remotePath);
    }

    /// <summary>
    /// Disconnects the active SSH and SCP connections.
    /// </summary>
    public void Close()
    {
        if (_sshClient?.IsConnected == true)
        {
            _sshClient.Disconnect();
        }
        if (_scpClient?.IsConnected == true)
        {
            _scpClient.Disconnect();
        }
    }
    
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
            _scpClient?.Dispose();
            _sshClient = null;
            _scpClient = null;
        }
        
        _disposed = true;
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
            throw new ObjectDisposedException(nameof(TerminalConnection), ObjectDisposedError);
        }
    }

    private void EnsureNotConnected()
    {
        if (_sshClient?.IsConnected == true || _scpClient?.IsConnected == true)
        {
            throw new InvalidOperationException(AlreadyConnectedError);
        }
    }

    private void EnsureConnected()
    {
        // Beide Clients (SSH für Befehle, SCP für Dateien) müssen verbunden sein.
        if (_sshClient?.IsConnected != true || _scpClient?.IsConnected != true)
        {
            throw new InvalidOperationException(NotConnectedError);
        }
    }
    
    private static string ExecuteAndValidateCommand(SshCommand cmd, string command)
    {
        var output = cmd.Execute();
        
        if (cmd.ExitStatus != 0)
        {
            // SshCommandException muss im Projekt existieren, wie im vorherigen Code-Set.
             throw new SshCommandException(
                $"Befehl '{command}' schlug fehl mit Exit Code {cmd.ExitStatus}. Fehler: {cmd.Error}",
                cmd.ExitStatus,
                cmd.Error
            );
        }
        
        return output;
    }
}