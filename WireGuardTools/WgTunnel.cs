namespace WireGuardTools;

public readonly record struct WgTunnel(WgPeer LocalPeer, WgPeer RemotePeer, WgPreSharedKey? PreSharedKey = null)
{
    public WgPeer LocalPeer { get; } = LocalPeer;
    public WgPeer RemotePeer { get; } = RemotePeer;
    public WgPreSharedKey PreSharedKey { get; } = PreSharedKey??new WgPreSharedKey();
}