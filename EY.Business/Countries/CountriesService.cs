using EY.Domain.Contracts;
using EY.Domain.Countries;
using EY.Domain.Models;
using EY.Shared.Attributes;

namespace EY.Business.Countries;

[BindInterface(typeof(ICountriesService))]
public class CountriesService : ICountriesService
{
    public const string CachePrefix = "Countries_";
    private readonly IRedisCache _redisCache;

    private readonly IUnitOfWork _unitOfWork;

    public CountriesService(IUnitOfWork unitOfWork, IRedisCache redisCache)
    {
        _unitOfWork = unitOfWork;
        _redisCache = redisCache;
    }

    public Result Add(CountryInput country)
    {
        var countryRepository = _unitOfWork.Repository<Country>();

        if (countryRepository.Get().Any(c => c.ThreeLetterCode == country.ThreeLetterCode))
            return Result.Failure($"Country with code '{country.ThreeLetterCode}' already exists.");

        var countryEntity = new Country
        {
            Name = country.Name,
            TwoLetterCode = country.TwoLetterCode,
            ThreeLetterCode = country.ThreeLetterCode
        };

        countryRepository.Add(countryEntity);
        _unitOfWork.Commit();

        var cacheKey = CachePrefix + country.ThreeLetterCode;
        _redisCache.AddAsync(cacheKey, country);

        return Result.Success();
    }

    public Result Delete(string threeLetterCode)
    {
        var countryRepository = _unitOfWork.Repository<Country>();

        var country = countryRepository.Get().SingleOrDefault(c => c.ThreeLetterCode == threeLetterCode);
        if (country is null)
            return CountriesResults.Errors.NotFound(threeLetterCode);

        countryRepository.Delete(country.Id);
        _unitOfWork.Commit();

        var cacheKey = CachePrefix + threeLetterCode;
        _redisCache.RemoveAsync(cacheKey);

        return CountriesResults.Deleted(country.Name);
    }

    public async Task<Result<Country>> Get(string threeLetterCode, CancellationToken cancellationToken = default)
    {
        var cacheKey = CachePrefix + threeLetterCode;

        var cachedCountry = await _redisCache.GetAsync<Country>(cacheKey, cancellationToken);
        if (cachedCountry is not null)
            return CountriesResults.FoundCache(cachedCountry);

        var countryRepository = _unitOfWork.Repository<Country>();

        var country = countryRepository.Get().FirstOrDefault(c => c.ThreeLetterCode == threeLetterCode);
        if (country is not null)
        {
            await _redisCache.AddAsync(cacheKey, country, cancellationToken);
            return CountriesResults.FoundDatabase(country);
        }

        return CountriesResults.Errors.NotFound(threeLetterCode);
    }

    public Result<PaginatedList<Country>> Get(PaginationInput pagination)
    {
        var countryRepository = _unitOfWork.Repository<Country>();

        var countries = countryRepository.Get();

        if (!string.IsNullOrWhiteSpace(pagination.Query))
            countries = countries.Where(c => c.Name.Contains(pagination.Query)
                                             || c.ThreeLetterCode.Contains(pagination.Query)
                                             || c.TwoLetterCode.Contains(pagination.Query));
        if (!countries.Any())
            return Result<PaginatedList<Country>>.Failure("No countries matched the query.");

        var paginatedCountries = new PaginatedList<Country>(countries, pagination.Index, pagination.Size);

        return Result<PaginatedList<Country>>.Success(paginatedCountries,
            $"A total of {paginatedCountries.TotalCount} countries in {paginatedCountries.TotalPages} pages.");
    }

    public Result Update(CountryInput country)
    {
        var countryRepository = _unitOfWork.Repository<Country>();

        var existingCountry = countryRepository.Get().FirstOrDefault(c => c.ThreeLetterCode == country.ThreeLetterCode);
        if (existingCountry == null)
            return CountriesResults.Errors.NotFound(country.ThreeLetterCode);

        existingCountry.Name = country.Name;
        existingCountry.TwoLetterCode = country.TwoLetterCode;
        existingCountry.ThreeLetterCode = country.ThreeLetterCode;

        _unitOfWork.Commit();

        var cacheKey = CachePrefix + country.ThreeLetterCode;
        _redisCache.AddAsync(cacheKey, existingCountry);

        return CountriesResults.Updated(country.Name);
    }
}