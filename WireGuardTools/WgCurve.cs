using System.Security.Cryptography;

namespace WireGuardTools.Curves;

/// <summary>
/// Provides functionality to generate WireGuard compatible key pairs using Curve25519.
/// </summary>
public static class WgCurve
{
    /// <summary>
    /// A static, reusable instance of the Curve25519 parameters.
    /// This avoids creating a new ECCurve object on every key generation call.
    /// </summary>
    public readonly static ECCurve Curve25519 = new ECCurve
    {
        CurveType = ECCurve.ECCurveType.PrimeMontgomery,
        Prime = WgCurve25519Constants.Prime,
        A = WgCurve25519Constants.A,
        B = WgCurve25519Constants.B,
        G = new ECPoint
        {
            X = WgCurve25519Constants.Gx,
            Y = WgCurve25519Constants.Gy,
        },
        Order = WgCurve25519Constants.Order,
        Cofactor = WgCurve25519Constants.Cofactor,
    };
}