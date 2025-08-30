# Third-Party Attributions

## Curve25519 Cryptographic Algorithm

**Designer:** Daniel J. Bernstein  
**Publication:** "Curve25519: new Diffie-Hellman speed records" (2006)  
**Specification:** RFC 7748 - Elliptic Curves for Security  
**Description:** Curve25519 is a Montgomery curve providing 128-bit security with high performance and resistance to various side-channel attacks.

**References:**
- Original paper: https://cr.yp.to/ecdh/curve25519-20060209.pdf
- RFC 7748: https://tools.ietf.org/html/rfc7748
- Author's website: https://cr.yp.to/

## Curve25519 .NET Implementation Parameters

**Source:** StackOverflow contributions  
**License:** MIT License  
**Authors:**
- Yasar_yy: Original Curve25519 ECCurve parameters implementation
- Deku Desu: ECDH usage example and improvements

**References:**
- https://stackoverflow.com/questions/65039528/why-does-curve25519-calculate-key-pair-correctly-even-though-its-parameters-are
- https://stackoverflow.com/questions/68622359/c-calculate-key-share-using-private-key-and-public-key-on-ecdhe-x25519

**Usage:** Curve25519 elliptic curve parameters for WireGuard key generation in .NET `ECDiffieHellman` implementation

## WireGuard Protocol

**Designers:** Jason A. Donenfeld and others  
**Specification:** WireGuard: Next Generation Kernel Network Tunnel  
**Description:** Modern VPN protocol designed for simplicity, performance, and security.

**References:**
- WireGuard whitepaper: https://www.wireguard.com/papers/wireguard.pdf
- Official website: https://www.wireguard.com/

**Usage:** This library generates keys compatible with the WireGuard protocol specification for secure VPN tunneling.