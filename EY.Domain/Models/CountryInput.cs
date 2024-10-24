using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models
{
    public record CountryInput([Required] string Name, [Required] string TwoLetterCode, [Required] string ThreeLetterCode)
    {
    }
}
