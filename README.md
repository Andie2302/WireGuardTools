# WireGuardTools

A .NET library for generating WireGuard-compatible Curve25519 key pairs using native .NET cryptography APIs.

## Features

- 🔐 **Native .NET Implementation** - Uses `ECDiffieHellmanCng` with Curve25519 parameters
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

var keyPair = WgKeyPairGenerator.CreateNewWgKeyPair();

// Access raw 32-byte arrays
byte[] privateKeyBytes = keyPair.PrivateKey;
byte[] publicKeyBytes = keyPair.PublicKey;

// Convert to Base64 manually
string privateKeyBase64 = keyPair.PrivateKeyAsBase64;
```

## API Reference

### WgKeyPairGenerator

#### Static Methods

| Method | Description | Returns |
|--------|-------------|---------|
| `CreateNewWgKeyPair()` | Generates a new WireGuard compatible key pair | `WgKeyPair` |
| `CreateMultipleKeyPairs(int count)` | Generates multiple key pairs efficiently | `IEnumerable<WgKeyPair>` |
| `KeyToBase64(byte[] key)` | Converts a key byte array to Base64 string | `string` |

### WgKeyPair

A readonly record struct representing a WireGuard key pair.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `PrivateKey` | `byte[]` | Raw private key (32 bytes) |
| `PublicKey` | `byte[]` | Raw public key (32 bytes) |
| `PrivateKeyAsBase64` | `string` | Private key as Base64 string |
| `PublicKeyAsBase64` | `string` | Public key as Base64 string |

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

- **.NET 6.0 or higher** (including Windows variants)
- Curve25519 support requires modern .NET runtime

### Supported Frameworks

- .NET 6.0
- .NET 6.0-windows
- .NET 7.0
- .NET 7.0-windows
- .NET 8.0
- .NET 8.0-windows
- .NET 9.0

## Performance

The library uses native Windows CNG (Cryptography Next Generation) APIs through `ECDiffieHellmanCng` for optimal performance:

- ⚡ **Fast key generation** - Leverages hardware acceleration when available
- 🔒 **Cryptographically secure** - Uses system entropy sources
- 💾 **Memory efficient** - Lazy enumeration for multiple key generation

## Security Considerations

- Keys are generated using cryptographically secure random number generators
- Private keys should be handled securely and cleared from memory when no longer needed
- The library follows RFC 7748 Curve25519 specifications
- No custom cryptographic implementations - relies on proven .NET Framework crypto APIs

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

This library uses Curve25519 elliptic curve parameters based on RFC 7748 and StackOverflow contributions. See [ATTRIBUTIONS.md](ATTRIBUTIONS.md) for detailed acknowledgments.

## Support

- 📖 **Documentation**: Full API documentation available in code comments
- 🐛 **Issues**: Report bugs or request features via GitHub Issues
- 💡 **Questions**: Use GitHub Discussions for general questions

---

**Made with ❤️ for the .NET and WireGuard communities**