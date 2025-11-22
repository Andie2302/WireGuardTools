using Renci.SshNet;

namespace WireGuardTools;

public sealed class KeyAuthenticationStrategy(string privateKeyPath, string passphrase) : IAuthenticationStrategy
{
    public string AuthenticationTypeSuffix => " (Key)";
        
    public SshClient CreateClient(ConnectionParameters parameters)
    {
        PrivateKeyFile? privateKeyFile = null;
        try
        {
            privateKeyFile = new PrivateKeyFile(privateKeyPath, passphrase);
            var connectionInfo = new ConnectionInfo(
                parameters.Host,
                parameters.Username,
                new PrivateKeyAuthenticationMethod(parameters.Username, privateKeyFile)
            );
            return new SshClient(connectionInfo);
        }
        catch
        {
            privateKeyFile?.Dispose();
            throw;
        }
    }
}