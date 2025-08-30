# WireGuardTools

A .NET library for generating WireGuard-compatible Curve25519 key pairs using native .NET cryptography APIs.

## Features

- 🔐 **Native .NET Implementation** - Uses `ECDiffieHellman.Create()` with standardized Curve25519 parameters
- 🌐 **Cross-Platform** - Works on Windows, Linux, and macOS without platform-specific dependencies
- 🚀 **No External Dependencies** - Pure .NET implementation without additional crypto libraries
- ✅ **WireGuard Compatible** - Generates keys that work seamlessly with the official WireGuard tools
- 🎯 **Multi-Target Support** - Compatible with .NET 6, 7, 8, and 9 (including Windows variants)
- 📝 **Well Documented** - Comprehensive XML documentation for all public APIs

## Quick Start

### Basic Key Generation

```csharp
using WireGuardTools.Generators;

// Generate a single key pair
var keyPair = WgKeyPairGenerator.CreateNewWgKeyPair();

Console.WriteLine($"Private Key: {keyPair.PrivateKeyAsBase64}");
Console.WriteLine($"Public Key:  {keyPair.PublicKeyAsBase64}");
```

### Generate Multiple Key Pairs

```csharp
using WireGuardTools.Generators;

// Generate multiple key pairs efficiently
foreach (var keyPair in WgKeyPairGenerator.CreateMultipleKeyPairs(5))
{
    Console.WriteLine($"Private: {keyPair.PrivateKeyAsBase64}");
    Console.WriteLine($"Public:  {keyPair.PublicKeyAsBase64}");
    Console.WriteLine();
}
```

### Working with Raw Bytes

```csharp
using WireGuardTools.Generators;
using WireGuardTools;

var keyPair = WgKeyPairGenerator.CreateNewWgKeyPair();

// Access raw 32-byte arrays
byte[] privateKeyBytes = keyPair.PrivateKey;
byte[] publicKeyBytes = keyPair.PublicKey;

// Convert to Base64 manually
string privateKeyBase64 = keyPair.PrivateKeyAsBase64;

// The key size is defined as a constant
Console.WriteLine($"Key size: {WgTools.KeySize} bytes");
```

## API Reference

### WgKeyPairGenerator

Located in `WireGuardTools.Generators` namespace.

#### Static Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `CreateNewWgKeyPair()` | Generates a new WireGuard compatible key pair | `WgKeyPair` |
| `CreateMultipleKeyPairs(int count)` | Generates multiple key pairs efficiently using yield return | `IEnumerable<WgKeyPair>` |

### WgKeyPair

A readonly record struct representing a WireGuard key pair with built-in validation.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `PrivateKey` | `byte[]` | Raw private key (32 bytes) |
| `PublicKey` | `byte[]` | Raw public key (32 bytes) |
| `PrivateKeyAsBase64` | `string` | Private key as Base64 string |
| `PublicKeyAsBase64` | `string` | Public key as Base64 string |

#### Constructor

```csharp
public WgKeyPair(byte[] privateKey, byte[] publicKey)
```

- Validates that both keys are exactly 32 bytes in length
- Throws `ArgumentNullException` if either key is null
- Throws `ArgumentException` if either key has incorrect length

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
# Generate a key pair with WireGuardTools
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
4. Return validated WgKeyPair structure

## Performance

The library uses the native .NET cryptography stack with consistent cross-platform behavior:

- ⚡ **Fast key generation** - Leverages platform-optimized implementations
- 🔒 **Cryptographically secure** - Uses system entropy sources
- 💾 **Memory efficient** - Lazy enumeration for multiple key generation using yield return
- 🌐 **Cross-platform consistent** - Same RFC 7748 parameters on all platforms
- ✅ **Input validation** - Runtime validation ensures key integrity

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
- Immutable key pair structure prevents accidental modification

## Architecture

```
WireGuardTools/
├── Generators/
│   ├── Constants/
│   │   └── WgCurve25519Constants.cs    # RFC 7748 curve parameters
│   └── WgKeyPairGenerator.cs           # Key generation logic
├── WgKeyPair.cs                        # Immutable key pair structure
└── WgTools.cs                          # Utility constants
```

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