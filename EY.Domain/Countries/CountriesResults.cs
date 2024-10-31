using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Countries
{
    public static class CountriesResults
    {
        public static class Errors
        {
            public static Result<Country> NotFound(string threeLetterCode) => Result<Country>.Failure($"Country '{threeLetterCode}' not found.");
        }
        public static Result<Country> FoundCache(Country country) => Result<Country>.Success(country, $"Country '{country.Name}' found in cache.");
        public static Result<Country> FoundDatabase(Country country) => Result<Country>.Success(country, $"Country '{country.Name}' found in database.");
        public static Result<Country> FoundExternally(Country country) => Result<Country>.Success(country, $"Country '{country.Name}' found externally.");


        public static Result Updated(string countryName) => Result.Success($"Country '{countryName}' updated successfully.");
        public static Result Deleted(string countryName) => Result.Success($"Country '{countryName}' deleted successfully.");



    }
}
