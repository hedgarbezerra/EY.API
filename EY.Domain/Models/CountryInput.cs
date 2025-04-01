using System.ComponentModel.DataAnnotations;

namespace EY.Domain.Models;

public record CountryInput([Required] string Name, [Required] string TwoLetterCode, [Required] string ThreeLetterCode)
{
}