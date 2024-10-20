using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models
{
    public record Ip2CResponse(string CountryName, string CountryTwoLetterCode, string CountryThreeLetterCode);
}
