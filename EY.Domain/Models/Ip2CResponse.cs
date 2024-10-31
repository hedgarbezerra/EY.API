using EY.Domain.IpAddresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models
{
    public record Ip2CResponse(string IpAddress, string CountryName, string CountryTwoLetterCode, string CountryThreeLetterCode)
    {
        public bool IsIpAddressUpdated(IpAddress ip) => ip?.Country?.Name != CountryName;
    }
}
