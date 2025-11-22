using Renci.SshNet;

namespace WireGuardTools;

public sealed class PasswordAuthenticationStrategy(string password) : IAuthenticationStrategy
{
    public string AuthenticationTypeSuffix => "";
        
    public SshClient CreateClient(ConnectionParameters parameters)
    {
        return new SshClient(parameters.Host, parameters.Port, parameters.Username, password);
    }
}