using System.Net;

namespace EY.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsValidIpAddress(this string ipAddress)
    {
        return IPAddress.TryParse(ipAddress, out _);
    }
}