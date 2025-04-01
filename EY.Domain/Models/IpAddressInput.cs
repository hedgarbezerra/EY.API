using System.ComponentModel.DataAnnotations;

namespace EY.Domain.Models;

public record IpAddressInput(
    [Required] string IpAddress,
    [Required] string CountryName,
    [Required] string CountryTwoLetterCode,
    [Required] string CountryThreeLetterCode)
{
}