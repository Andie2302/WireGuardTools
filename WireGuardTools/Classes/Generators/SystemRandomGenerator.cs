using WireGuardTools.Interfaces.Generators;

namespace WireGuardTools.Classes.Generators;

public sealed class SystemRandomGenerator : IRandomGenerator
{
    public void Fill ( byte[] buffer ) => System.Security.Cryptography.RandomNumberGenerator.Fill ( buffer );
    public byte[] GetBytes ( int count ) => System.Security.Cryptography.RandomNumberGenerator.GetBytes ( count );
}
