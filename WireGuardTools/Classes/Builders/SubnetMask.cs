using System.Net;

namespace WireGuardTools.Classes.Builders;

public record struct SubnetMask ( int PrefixLength )
{
    public IPAddress Mask { get; private set; }
    public override string ToString() => Mask.ToString();
}