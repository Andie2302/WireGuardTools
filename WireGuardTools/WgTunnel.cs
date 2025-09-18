namespace WireGuardTools;

public class WgTunnel(WgKeyPair serverToClient, WgKeyPair clientToServer, WgKey preSharedKey)
{
    public WgKeyPair ServerToClient { get; } = serverToClient ?? throw new ArgumentNullException(nameof(serverToClient));
    public WgKeyPair ClientToServer { get; } = clientToServer ?? throw new ArgumentNullException(nameof(clientToServer));
    public WgKey PreSharedKey { get; } = preSharedKey ?? throw new ArgumentNullException(nameof(preSharedKey));
    public override string ToString() => $"ServerToClient: {ServerToClient}, ClientToServer: {ClientToServer}, PreSharedKey: {PreSharedKey}";
}