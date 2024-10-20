using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidIpAddress(this string ipAddress) =>  System.Net.IPAddress.TryParse(ipAddress, out _);
    }
}
