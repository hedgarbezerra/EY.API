using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models
{
    public record IpAddressReportItem(string CountryName, int IpAddressesCount, DateTime LastIpAddressUpdatedAt);
}
