namespace WireGuardTools;

public class WgTunnel(WgKeyPair serverToClient, WgKeyPair clientToServer, WgKey preSharedKey)
{
    public WgKeyPair ServerToClient { get; } = serverToClient ?? throw new ArgumentNullException(nameof(serverToClient));
    public WgKeyPair ClientToServer { get; } = clientToServer ?? throw new ArgumentNullException(nameof(clientToServer));
    public WgKey PreSharedKey { get; } = preSharedKey ?? throw new ArgumentNullException(nameof(preSharedKey));
    public override string ToString() => $"ServerToClient:\t{ServerToClient}\nClientToServer:\t{ClientToServer}\nPreSharedKey:\t{PreSharedKey}";
    public static WgTunnel Create(WgKeyPair serverToClient, WgKeyPair clientToServer, WgKey preSharedKey) => new(serverToClient, clientToServer, preSharedKey);
    public static WgTunnel CreateRandom(IWgKeyPairGenerator keyPairGenerator) => Create(keyPairGenerator.GenerateKeyPair(), keyPairGenerator.GenerateKeyPair(), WgKey.CreateRandom());
    public static WgTunnel CreateRandom() => CreateRandom(new Curve25519KeyPairGenerator());
    public static IEnumerable<WgTunnel> CreateRandom(IWgKeyPairGenerator keyPairGenerator, int count) => Enumerable.Range(0, count).Select(_ => CreateRandom(keyPairGenerator));
    public static IEnumerable<WgTunnel> CreateRandom(int count) => CreateRandom(new Curve25519KeyPairGenerator(), count);
}
