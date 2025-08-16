using WireGuardTools.Classes.Base;

namespace WireGuardTools.Interfaces.Generators;

public interface IWgPresharedKeyGenerator
{
    IRandomGenerator RandomGenerator { get; init; }
    WgBaseKey GeneratePresharedKey();
}