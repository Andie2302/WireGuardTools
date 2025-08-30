namespace WireGuardTools;

public readonly record struct WgTunnel
{
    public WgTunnel(WgPeer? LocalPeer = null, WgPeer? RemotePeer = null, WgKey? PreSharedKey = null)
    {
        this.LocalPeer = LocalPeer ?? WgPeer.CreateRandom();
        this.RemotePeer = RemotePeer ?? WgPeer.CreateRandom();
        this.PreSharedKey = PreSharedKey ?? WgKey.CreateRandom();
    }

    public WgPeer LocalPeer { get; init; }
    public WgPeer RemotePeer { get; init; }
    public WgKey? PreSharedKey { get; init; }

    public void Deconstruct(out WgPeer localPeer, out WgPeer remotePeer, out WgKey? preSharedKey)
    {
        localPeer = this.LocalPeer;
        remotePeer = this.RemotePeer;
        preSharedKey = this.PreSharedKey;
    }
}