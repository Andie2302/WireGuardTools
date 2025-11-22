using Renci.SshNet;

namespace WireGuardTools;

public interface IAuthenticationStrategy
{
    string AuthenticationTypeSuffix { get; }
    SshClient CreateClient(ConnectionParameters parameters);
}