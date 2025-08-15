namespace WireGuardTools.Interfaces.Generators;

public interface IRandomGenerator
{
    void Fill ( byte[] buffer );
    byte[] GetBytes ( int count );
}
