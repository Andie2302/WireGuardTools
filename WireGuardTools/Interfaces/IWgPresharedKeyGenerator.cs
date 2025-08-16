using WireGuardTools.Classes.Base;

namespace WireGuardTools.Interfaces;

public interface IWgPresharedKeyGenerator
{
    IRandomGenerator RandomGenerator { get; init; }
    WgBaseKey GeneratePresharedKey();
}