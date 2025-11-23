using Renci.SshNet.Common;

namespace WireGuardTools;

/// <summary>
/// Die öffentliche Schnittstelle zur Durchführung von Remote-Operationen.
/// Nutzt statische Methoden, um Verbindungen klar zu definieren und die Lebensdauer über IDisposable zu verwalten.
/// </summary>
public sealed class Terminal : IDisposable
{
    private readonly TerminalConnection _connection;

    /// <summary>
    /// Privater Konstruktor, der die konfigurierte Verbindung entgegennimmt.
    /// </summary>
    /// <param name="connection">Die bereits erstellte und konfigurierte Verbindung.</param>
    private Terminal(TerminalConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// Erstellt eine neue Terminal-Instanz für eine kennwortbasierte Verbindung.
    /// </summary>
    public static Terminal CreateWithPassword(string host, string username, string password, int port = 22)
    {
        var settings = new PasswordConnectionSettings 
        { 
            Host = host, 
            Port = port, 
            Username = username, 
            Password = password 
        };
        var connection = new TerminalConnection(settings);
        return new Terminal(connection);
    }

    /// <summary>
    /// Erstellt eine neue Terminal-Instanz für eine schlüsselbasierte Verbindung.
    /// </summary>
    public static Terminal CreateWithKey(string host, string username, string privateKeyPath, string passphrase, int port = 22)
    {
        var settings = new KeyConnectionSettings 
        { 
            Host = host, 
            Port = port, 
            Username = username, 
            PrivateKeyPath = privateKeyPath, 
            Passphrase = passphrase 
        };
        var connection = new TerminalConnection(settings);
        return new Terminal(connection);
    }

    /// <summary>
    /// Baut die Verbindung zum Remote-Host auf.
    /// </summary>
    public void Connect() => _connection.Connect();
    
    /// <summary>
    /// Führt einen Befehl auf dem Remote-Host aus.
    /// </summary>
    public string ExecuteCommand(string command) => _connection.ExecuteCommand(command);
    
    /// <summary>
    /// Lädt eine Datei vom Remote-Host herunter.
    /// </summary>
    public void DownloadFile(string remotePath, string localPath) => _connection.DownloadFile(remotePath, localPath);
    
    /// <summary>
    /// Lädt eine Datei zum Remote-Host hoch.
    /// </summary>
    public void UploadFile(string localPath, string remotePath) => _connection.UploadFile(localPath, remotePath);

    /// <summary>
    /// Gibt alle von der Terminal-Instanz verwendeten Ressourcen frei, einschließlich der Verbindung.
    /// </summary>
    public void Dispose()
    {
        _connection.Dispose();
    }
}