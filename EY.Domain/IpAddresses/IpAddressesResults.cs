using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.IpAddresses
{
    public static class IpAddressesResults
    {
        public static class Errors
        {
            public static Result<Ip2CResponse> IpNotFound(string ipAddress) => Result<Ip2CResponse>.Failure($"IP address '{ipAddress}' not found.");
            public static Result<Ip2CResponse> IpNotValid(string ipAddress) => Result<Ip2CResponse>.Failure($"IP address '{ipAddress}' is not valid.");
        }

        public static Result<Ip2CResponse> FoundCache(Ip2CResponse ipResponse) => Result<Ip2CResponse>.Success(ipResponse, $"IP address '{ipResponse.IpAddress}' found in cache.");
        public static Result<Ip2CResponse> FoundDatabase(Ip2CResponse ipResponse) => Result<Ip2CResponse>.Success(ipResponse, $"IP address '{ipResponse.IpAddress}' found in database.");
        public static Result<Ip2CResponse> FoundExternally(Ip2CResponse ipResponse) => Result<Ip2CResponse>.Success(ipResponse, $"IP address '{ipResponse.IpAddress}' found externally.");

        public static Result Updated(string ipAddress) => Result.Success($"IP address '{ipAddress} ' updated successfully.");
        public static Result Deleted(string ipAddress) => Result.Success($"IP address '{ipAddress}' deleted successfully.");
    }
}
