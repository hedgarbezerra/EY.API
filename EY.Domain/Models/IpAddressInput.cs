using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models
{
    public record IpAddressInput([Required] string IpAddress, [Required] string CountryName, [Required] string CountryTwoLetterCode, [Required] string CountryThreeLetterCode)
    {
    }
}
