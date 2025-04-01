using EY.Domain.Models;

namespace EY.Domain.Countries;

public static class CountriesResults
{
    public static Result<Country> FoundCache(Country country)
    {
        return Result<Country>.Success(country, $"Country '{country.Name}' found in cache.");
    }

    public static Result<Country> FoundDatabase(Country country)
    {
        return Result<Country>.Success(country, $"Country '{country.Name}' found in database.");
    }

    public static Result<Country> FoundExternally(Country country)
    {
        return Result<Country>.Success(country, $"Country '{country.Name}' found externally.");
    }


    public static Result Updated(string countryName)
    {
        return Result.Success($"Country '{countryName}' updated successfully.");
    }

    public static Result Deleted(string countryName)
    {
        return Result.Success($"Country '{countryName}' deleted successfully.");
    }

    public static class Errors
    {
        public static Result<Country> NotFound(string threeLetterCode)
        {
            return Result<Country>.Failure($"Country '{threeLetterCode}' not found.");
        }
    }
}