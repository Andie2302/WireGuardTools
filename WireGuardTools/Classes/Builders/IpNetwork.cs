using System.Net;

namespace WireGuardTools.Classes.Builders;

public class IpNetwork
{
    public readonly IPAddress Address;
    public readonly IPAddress Mask;

    public IpNetwork ( IPAddress address , IPAddress mask )
    {
        Address = address;
        Mask = mask;
    }
}