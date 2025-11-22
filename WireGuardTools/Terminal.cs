namespace WireGuardTools;

/// <summary>
/// Die öffentliche Schnittstelle zur Durchführung von Remote-Operationen.
/// Nutzt statische Methoden, um Verbindungen klar zu definieren und die Lebensdauer über IDisposable zu verwalten.
/// </summary>
public sealed class Terminal : IDisposable
{
    private readonly Terminal _connection;

    private Terminal(ConnectionSettings settings, Terminal connection)
    {
        _connection = connection;
    }

    public static Terminal CreateWithPassword(string host, string username, string password, int port = 22)
    {
        var settings = new PasswordConnectionSettings 
        { 
            Host = host, 
            Port = port, 
            Username = username, 
            Password = password 
        };
        return new Terminal(settings);
    }

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
        return new Terminal(settings);
    }

    public void Connect() => _connection.Connect();
    public string ExecuteCommand(string command) => _connection.ExecuteCommand(command);
    public void DownloadFile(string remotePath, string localPath) => _connection.DownloadFile(remotePath, localPath);
    public void UploadFile(string localPath, string remotePath) => _connection.UploadFile(localPath, remotePath);

    public void Dispose()
    {
        _connection.Dispose();
    }
}