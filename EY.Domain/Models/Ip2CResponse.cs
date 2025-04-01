using EY.Domain.IpAddresses;

namespace EY.Domain.Models;

public record Ip2CResponse(
    string IpAddress,
    string CountryName,
    string CountryTwoLetterCode,
    string CountryThreeLetterCode)
{
    public bool IsIpAddressUpdated(IpAddress ip)
    {
        return ip?.Country?.Name != CountryName;
    }
}