# WireGuardTools

A modern .NET library for generating WireGuard-compatible Curve25519 key pairs and managing tunnel configurations using native .NET cryptography APIs.

## ✨ Features

- 🔐 **Native .NET Implementation** - Uses standardized RFC 7748 Curve25519 parameters with `ECDiffieHellman`
- 🌐 **Cross-Platform** - Works consistently on Windows, Linux, and macOS
- 🚀 **Zero Dependencies** - Pure .NET implementation without external crypto libraries
- ✅ **WireGuard Compatible** - Generates keys that work seamlessly with official WireGuard tools
- 🎯 **Multi-Target Support** - Compatible with .NET 6-9 (including Windows variants)
- 📝 **Comprehensive Documentation** - Full XML documentation for all public APIs
- 🛡️ **Type-Safe & Immutable** - Modern C# record structs with defensive copying
- ⚡ **High Performance** - Efficient key generation with yield return patterns

## 🚀 Quick Start

### Basic Key Generation

```csharp
using WireGuardTools;

// Generate a secure random key
var key = WgKey.CreateRandom();
Console.WriteLine($"Key: {key.KeyAsBase64}");

// Create from existing data
var keyFromBase64 = WgKey.Create("eLyH7Dze4G8wceQKFmGnWJ6Dv2zAfgSLbxwN5UlzsWc=");
var keyFromBytes = WgKey.Create(myByteArray);
```

### Generate Key Pairs (Peers)

```csharp
using WireGuardTools;

// Generate a single peer (private/public key pair)
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

// Create tunnel with specific configuration
var localPeer = WgPeer.CreateRandom();
var remotePeer = WgPeer.CreateRandom();
var preSharedKey = WgKey.CreateRandom();
var customTunnel = new WgTunnel(localPeer, remotePeer, preSharedKey);

// Deconstruct for easy access
var (local, remote, psk) = customTunnel;
```

### Safe Logging & Security

```csharp
using WireGuardTools;

var key = WgKey.CreateRandom();

// Safe for production logs - shows only first 8 characters
Console.WriteLine(key.ToString()); 
// Output: WgKey[eLyH7Dze...]

// Full key (use with caution in production)
Console.WriteLine(key.ToFullString()); 
// Output: WgKey[eLyH7Dze4G8wceQKFmGnWJ6Dv2zAfgSLbxwN5UlzsWc=]
```

## 📚 API Reference

### WgKey

Universal WireGuard key type for any 32-byte cryptographic key with immutable design.

#### Static Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `Create(byte[] key)` | Creates from byte array | `WgKey` |
| `Create(string base64Key)` | Creates from Base64 string | `WgKey` |
| `CreateRandom()` | Generates cryptographically secure random key | `WgKey` |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Key` | `byte[]` | Raw key bytes (32 bytes, defensive copy) |
| `KeyAsBase64` | `string` | Base64 encoded key |

#### Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `ToString()` | Safe representation for logging (truncated) | `string` |
| `ToFullString()` | Complete key representation (use with caution) | `string` |

### WgPeer

Represents a WireGuard peer with Curve25519 key pair (immutable record struct).

#### Static Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `CreateRandom()` | Generates new random key pair | `WgPeer` |

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `PrivateKey` | `byte[]` | Raw private key (32 bytes, defensive copy) |
| `PublicKey` | `byte[]` | Raw public key (32 bytes, defensive copy) |
| `PrivateKeyAsBase64` | `string` | Private key as Base64 |
| `PublicKeyAsBase64` | `string` | Public key as Base64 |

#### Constructor

```csharp
public WgPeer(byte[] privateKey, byte[] publicKey)
```

### WgPeerGenerator

Efficient utility for generating multiple peers with memory-conscious patterns.

#### Static Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `Create()` | Generates single peer | `WgPeer` |
| `Create(int count)` | Generates multiple peers (yield return) | `IEnumerable<WgPeer>` |

### WgTunnel

Complete WireGuard tunnel configuration (immutable record struct).

#### Constructor

```csharp
public WgTunnel(WgPeer? LocalPeer = null, WgPeer? RemotePeer = null, WgKey? PreSharedKey = null)
```

All parameters optional - random keys generated automatically if not provided.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `LocalPeer` | `WgPeer` | Local peer (server/client) |
| `RemotePeer` | `WgPeer` | Remote peer (client/server) |
| `PreSharedKey` | `WgKey?` | Optional pre-shared key for enhanced security |

#### Deconstruction

```csharp
var (local, remote, psk) = tunnel;
```

### WgTools

Utility constants for WireGuard operations.

#### Constants

| Constant | Value | Description |
|----------|-------|-------------|
| `KeySize` | `32` | Standard WireGuard key size in bytes |


### PackageReference
```xml
<PackageReference Include="WireGuardTools" Version="0.0.1" />
```

## ✅ Compatibility & Verification

This library generates keys fully compatible with:

- ✅ Official WireGuard tools (`wg` command-line)
- ✅ WireGuard-Go implementation
- ✅ WireGuard kernel module
- ✅ Any RFC 7748 compliant Curve25519 implementation

### Verification Example

Test compatibility with official WireGuard tools:

```bash
# Generate a key with WireGuardTools
# Private: eLyH7Dze4G8wceQKFmGnWJ6Dv2zAfgSLbxwN5UlzsWc=

# Verify with official wg command
echo "eLyH7Dze4G8wceQKFmGnWJ6Dv2zAfgSLbxwN5UlzsWc=" | wg pubkey
# Output: GL16o84YrQrHy7Ew7bmwDPM27MnilJ/y7bG4wyhUuGo=
```

## 🎯 Requirements & Support

### Minimum Requirements
- **.NET 6.0 or higher** - Cross-platform Curve25519 support
- RFC 7748 compliant runtime environment

### Supported Target Frameworks
- .NET 6.0 / 6.0-windows
- .NET 7.0 / 7.0-windows
- .NET 8.0 / 8.0-windows
- .NET 9.0

### Platform Support

| Platform | Implementation | Optimization |
|----------|---------------|--------------|
| **Windows** | CNG (Cryptography Next Generation) | Hardware acceleration when available |
| **Linux** | OpenSSL | Native performance with system libraries |
| **macOS** | Security Framework | Optimized for Apple Silicon & Intel |

*All platforms use identical RFC 7748 parameters ensuring consistent behavior.*

## 🏗️ Implementation Details

### Cryptographic Foundation

Uses explicit RFC 7748 Curve25519 parameters:

- **Curve Type**: Prime Montgomery curve
- **Prime Field**: 2^255 - 19
- **Generator Point**: Standardized base point
- **Order**: Curve order for cryptographic operations
- **Cofactor**: 8 (RFC 7748 specification)

### Key Generation Process

1. Create `ECDiffieHellman` instance with explicit Curve25519 parameters
2. Export key parameters (private key D, public key Q.X)
3. Validate and ensure proper 32-byte length with padding if necessary
4. Return validated immutable structures with defensive copying

## ⚡ Performance & Design

### Performance Characteristics
- **Fast Generation**: Platform-optimized native implementations
- **Memory Efficient**: Lazy enumeration with yield return patterns
- **Cryptographically Secure**: System entropy sources
- **Cross-Platform Consistent**: Identical RFC 7748 behavior

### Security & Design Principles
- **Type Safety**: Immutable record structs prevent accidental modification
- **Input Validation**: Runtime validation ensures key integrity
- **Defensive Copying**: Prevents external mutation of internal key data
- **Safe Logging**: Built-in methods prevent accidental key exposure
- **Zero Trust**: All inputs validated, no assumptions about external data

## 💡 Usage Patterns

### Simple Key Management
```csharp
// Individual keys for basic configuration
var serverKey = WgKey.CreateRandom();
var clientKey = WgKey.CreateRandom();
```

### Peer-to-Peer VPN Setup
```csharp
// Two peers for direct P2P connection
var peer1 = WgPeer.CreateRandom();
var peer2 = WgPeer.CreateRandom();
```

### Hub-and-Spoke Architecture
```csharp
// Server with multiple client peers
var hubServer = WgPeer.CreateRandom();
var clients = WgPeerGenerator.Create(50).ToList();
```

### Enterprise Tunnel Management
```csharp
// Complete tunnel with enhanced security
var tunnel = new WgTunnel(
    LocalPeer: serverPeer,
    RemotePeer: clientPeer,
    PreSharedKey: WgKey.CreateRandom() // Optional PSK for post-quantum resistance
);
```

## 📦 Project Structure

```
WireGuardTools/
├── Generators/
│   ├── Constants/
│   │   └── WgCurve25519Constants.cs    # RFC 7748 curve parameters
│   └── WgPeerGenerator.cs              # Efficient key pair generation
├── WgKey.cs                            # Universal immutable key type
├── WgPeer.cs                           # Immutable key pair structure  
├── WgTunnel.cs                         # Complete tunnel configuration
└── WgTools.cs                          # Utility constants
```

## 🔒 Security Considerations

- **Key Generation**: Uses cryptographically secure system RNG
- **Memory Safety**: Defensive copying prevents external key mutation
- **Immutable Design**: Record structs prevent accidental modification
- **Input Validation**: All constructors validate key parameters
- **Safe Logging**: Built-in truncation prevents accidental exposure
- **Cross-Platform**: Consistent RFC 7748 parameters across all platforms
- **Zero Dependencies**: Reduces attack surface, uses only .NET BCL

## 🤝 Contributing

Contributions welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all existing tests pass
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Daniel J. Bernstein** - Designer of Curve25519
- **StackOverflow Contributors** - Yasar_yy and Deku Desu for .NET ECCurve parameters
- **WireGuard Team** - For the excellent VPN protocol design

See [ATTRIBUTIONS.md](ATTRIBUTIONS.md) for detailed acknowledgments and references.

## 📞 Support & Community

- 📖 **Documentation**: Comprehensive XML docs in source code
- 🐛 **Issues**: [GitHub Issues](../../issues) for bugs and feature requests
- 💡 **Discussions**: [GitHub Discussions](../../discussions) for questions
- 📧 **Security**: Report security issues privately via GitHub Security

---

**Made with ❤️ for the .NET and WireGuard communities**

*Secure, fast, and reliable WireGuard key generation for modern .NET applications*