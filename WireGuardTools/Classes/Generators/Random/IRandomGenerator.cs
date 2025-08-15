namespace WireGuardTools.Classes.Generators.Random;

public interface IRandomGenerator
{
    void Fill ( byte[] buffer );
    byte[] GetBytes ( int count );
}
