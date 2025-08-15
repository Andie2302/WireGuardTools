namespace WireGuardTools.ClassesOld.Generators.Random;

public interface IRandomGenerator
{
    void Fill ( byte[] buffer );
    byte[] GetBytes ( int count );
}
