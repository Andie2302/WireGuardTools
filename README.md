# WireGuardTools

A .NET library for generating WireGuard-compatible Curve25519 key pairs and managing WireGuard configurations using native .NET cryptography APIs.

## Features

- 🔐 **Native .NET Implementation** - Uses `ECDiffieHellman.Create()` with standardized Curve25519 parameters
- 🌐 **Cross-Platform** - Works on Windows, Linux, and macOS without platform-specific dependencies
- 🚀 **No External Dependencies** - Pure .NET implementation without additional crypto libraries
- ✅ **WireGuard Compatible** - Generates keys that work seamlessly with the official WireGuard tools
- 🎯 **Multi-Target Support** - Compatible with .NET 6, 7, 8, and 9 (including Windows variants)
- 📝 **Well Documented** - Comprehensive XML documentation for all public APIs
- 🔑 **Flexible Key Management** - Support for individual keys, key pairs, and complete tunnel configurations

## Quick Start

### Basic Key Generation

```csharp
using WireGuardTools;

// Generate a random key
var key = WgKey.CreateRandom();
Console.WriteLine($"Key: {key.KeyAsBase64}");

// Create key from existing data
var keyFromBase64 = WgKey.Create("eLyH7Dze4G8wceQKFmGnWJ6Dv2zAfgSLbxwN5UlzsWc=");
var keyFromBytes = WgKey.Create(myByteArray);
```

### Generate Key Pairs (Peers)

```csharp
using WireGuardTools;

// Generate a single peer (key pair)
var peer = WgPeer.CreateRandom();
Console.WriteLine($"Private Key: {peer.PrivateKeyAsBase64}");
Console.WriteLine($"Public Key:  {peer.PublicKeyAsBase64}");

// Generate multiple peers efficiently
foreach (var peer in WgPeerGenerator.Create(5))
{
    Console.WriteLine($"Private: {peer.PrivateKeyAsBase64}");
    Console.WriteLine($"Public:  {peer.PublicKeyAsBase64}");
    Console.WriteLine();
}
```

### Complete Tunnel Configuration

```csharp
using WireGuardTools;

// Create a complete tunnel with random keys
var tunnel = new WgTunnel();
Console.WriteLine($"Local Private:  {tunnel.LocalPeer.PrivateKeyAsBase64}");
Console.WriteLine($"Local Public:   {tunnel.LocalPeer.PublicKeyAsBase64}");
Console.WriteLine($"Remote Private: {tunnel.RemotePeer.PrivateKeyAsBase64}");
Console.WriteLine($"Remote Public:  {tunnel.RemotePeer.PublicKeyAsBase64}");
Console.WriteLine($"Pre-shared Key: {tunnel.PreSharedKey?.KeyAsBase64}");

// Create tunnel with specific peers
var localPeer = WgPeer.CreateRandom();
var remotePeer = WgPeer.CreateRandom();
var preSharedKey = WgKey.CreateRandom();
var customTunnel = new WgTunnel(localPeer, remotePeer, preSharedKey);

// Deconstruct tunnel
var (local, remote, psk) = customTunnel;
```

### Safe Logging

```csharp
using WireGuardTools;

var key = WgKey.CreateRandom();

// Safe for logs - shows only first 8 characters
Console.WriteLine(key.ToString()); // WgKey[eLyH7Dze...]

// Full key (use with caution)
Console.WriteLine(key.ToFullString()); // WgKey[eLyH7Dze4G8wceQKFmGnWJ6Dv2zAfgSLbxwN5UlzsWc=]
```

## API Reference

### WgKey

A universal WireGuard key type for any 32-byte cryptographic key.

#### Static Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `Create(byte[] key)` | Creates a WgKey from byte array | `WgKey` |
| `Create(string base64Key)` | Creates a WgKey from Base64 string | `WgKey` |
| `CreateRandom()` | Generates a cryptographically secure random key | `WgKey` |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Key` | `byte[]` | Raw key bytes (32 bytes) |
| `KeyAsBase64` | `string` | Key as Base64 string |

#### Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `ToString()` | Safe string representation for logging | `string` |
| `ToFullString()` | Complete key representation (use with caution) | `string` |

### WgPeer

Represents a WireGuard peer with private and public key pair.

#### Static Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `CreateRandom()` | Generates a new random key pair | `WgPeer` |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `PrivateKey` | `byte[]` | Raw private key (32 bytes) |
| `PublicKey` | `byte[]` | Raw public key (32 bytes) |
| `PrivateKeyAsBase64` | `string` | Private key as Base64 string |
| `PublicKeyAsBase64` | `string` | Public key as Base64 string |

#### Constructor

```csharp
public WgPeer(byte[] privateKey, byte[] publicKey)
```

### WgPeerGenerator

Utility class for generating multiple peers efficiently.

#### Static Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `Create()` | Generates a single peer | `WgPeer` |
| `Create(int count)` | Generates multiple peers with yield return | `IEnumerable<WgPeer>` |

### WgTunnel

Represents a complete WireGuard tunnel configuration.

#### Constructor

```csharp
public WgTunnel(WgPeer? LocalPeer = null, WgPeer? RemotePeer = null, WgKey? PreSharedKey = null)
```

- All parameters are optional
- If not provided, random keys/peers are generated automatically

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `LocalPeer` | `WgPeer` | Local peer (server/client) |
| `RemotePeer` | `WgPeer` | Remote peer (client/server) |
| `PreSharedKey` | `WgKey?` | Optional pre-shared key for additional security |

#### Methods

```csharp
// Deconstruction support
var (local, remote, psk) = tunnel;
```

### WgTools

Utility class with constants for WireGuard operations.

#### Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `KeySize` | `32` | Size in bytes of WireGuard keys |

## Compatibility

This library generates keys that are fully compatible with:
- ✅ Official WireGuard tools (`wg` command)
- ✅ WireGuard-Go
- ✅ WireGuard kernel module
- ✅ Any RFC 7748 compliant Curve25519 implementation

### Verification Example

You can verify the generated keys work with the official WireGuard tools:

```bash
# Generate a key with WireGuardTools
# Private: eLyH7Dze4G8wceQKFmGnWJ6Dv2zAfgSLbxwN5UlzsWc=

# Verify with wg command 
echo "eLyH7Dze4G8wceQKFmGnWJ6Dv2zAfgSLbxwN5UlzsWc=" | wg pubkey
# Output: GL16o84YrQrHy7Ew7bmwDPM27MnilJ/y7bG4wyhUuGo=
```

## Requirements

- **.NET 6.0 or higher** - Cross-platform support with consistent Curve25519 implementation
- Standardized RFC 7748 Curve25519 parameters ensure compatibility across platforms

### Supported Frameworks

- .NET 6.0
- .NET 6.0-windows
- .NET 7.0
- .NET 7.0-windows
- .NET 8.0
- .NET 8.0-windows
- .NET 9.0

## Implementation Details

The library uses explicit Curve25519 parameters as defined in RFC 7748:

- **Curve Type**: Prime Montgomery curve
- **Prime Field**: 2^255 - 19
- **Generator Point**: Standardized base point for key generation
- **Order**: Curve order for cryptographic operations
- **Cofactor**: 8 (as specified in RFC 7748)

Key generation follows the standard process:
1. Create ECDiffieHellman instance with Curve25519 parameters
2. Export key parameters including private key (D) and public key (Q.X)
3. Ensure proper key length (32 bytes) with padding if necessary
4. Return validated structures with immutable design

## Performance

The library uses the native .NET cryptography stack with consistent cross-platform behavior:

- ⚡ **Fast key generation** - Leverages platform-optimized implementations
- 🔒 **Cryptographically secure** - Uses system entropy sources
- 💾 **Memory efficient** - Lazy enumeration for multiple key generation using yield return
- 🌐 **Cross-platform consistent** - Same RFC 7748 parameters on all platforms
- ✅ **Input validation** - Runtime validation ensures key integrity
- 🛡️ **Immutable design** - Defensive copying prevents accidental key modification

## Platform Support

| Platform | Implementation | Notes |
|----------|---------------|-------|
| **Windows** | CNG (Cryptography Next Generation) | Hardware acceleration when available |
| **Linux** | OpenSSL | Native performance with system crypto libraries |
| **macOS** | Security Framework | Optimized for Apple silicon and Intel |

*All platforms use standardized RFC 7748 Curve25519 parameters for consistent behavior.*

## Security Considerations

- Keys are generated using cryptographically secure random number generators
- Private keys should be handled securely and cleared from memory when no longer needed
- The library uses standardized RFC 7748 Curve25519 parameters for consistent behavior
- Uses proven platform-native cryptographic implementations
- Cross-platform compatibility ensured through explicit parameter specification
- Built-in validation prevents malformed keys from being created
- Immutable structures with defensive copying prevent accidental modification
- Safe logging methods prevent accidental key exposure in logs

## Architecture

```
WireGuardTools/
├── Generators/
│   ├── Constants/
│   │   └── WgCurve25519Constants.cs    # RFC 7748 curve parameters
│   └── WgPeerGenerator.cs              # Key pair generation logic
├── WgKey.cs                            # Universal key type
├── WgPeer.cs                           # Key pair (peer) structure
├── WgTunnel.cs                         # Complete tunnel configuration
├── WgPreSharedKey.cs                   # Legacy pre-shared key type
└── WgTools.cs                          # Utility constants
```

## Use Cases

### Simple Key Management
```csharp
// Individual keys for configuration
var serverKey = WgKey.CreateRandom();
var clientKey = WgKey.CreateRandom();
```

### Peer-to-Peer Setup
```csharp
// Generate two peers for P2P connection
var peer1 = WgPeer.CreateRandom();
var peer2 = WgPeer.CreateRandom();
```

### Server with Multiple Clients
```csharp
var server = WgPeer.CreateRandom();
var clients = WgPeerGenerator.Create(10).ToList();
```

### Complete Tunnel Setup
```csharp
// Ready-to-use tunnel configuration
var tunnel = new WgTunnel();
// All keys generated automatically
```

## Migration from Earlier Versions

If upgrading from earlier versions:

- `WgKeyPair` → Use `WgPeer`
- `WgKeyPairGenerator` → Use `WgPeerGenerator`
- Individual keys → Use `WgKey` instead of raw byte arrays
- Pre-shared keys → Use `WgKey` instead of `WgPreSharedKey`

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

This library uses Curve25519 elliptic curve parameters based on RFC 7748 and StackOverflow contributions. Special thanks to Daniel J. Bernstein for designing Curve25519. See [ATTRIBUTIONS.md](ATTRIBUTIONS.md) for detailed acknowledgments.

## Support

- 📖 **Documentation**: Full API documentation available in code comments
- 🐛 **Issues**: Report bugs or request features via GitHub Issues
- 💡 **Questions**: Use GitHub Discussions for general questions

---

**Made with ❤️ for the .NET and WireGuard communities**