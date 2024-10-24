using EY.Domain.Contracts;
using EY.Domain.Entities;
using EY.Domain.Models;
using EY.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Business.Services
{
    [BindInterface(typeof(ICountriesService))]
    public class CountriesService : ICountriesService
    {
        public const string CachePrefix = "Countries_";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCache _redisCache;

        public CountriesService(IUnitOfWork unitOfWork, IRedisCache redisCache)
        {
            _unitOfWork = unitOfWork;
            _redisCache = redisCache;
        }

        public Result Add(CountryInput country)
        {
            var countryRepository = _unitOfWork.Repository<Country>();

            if (countryRepository.Get().Any(c => c.ThreeLetterCode == country.ThreeLetterCode))
                return Result.Failure([$"Country with code '{country.ThreeLetterCode}' already exists."]);

            var countryEntity = new Country
            {
                Name = country.Name,
                TwoLetterCode = country.TwoLetterCode,
                ThreeLetterCode = country.ThreeLetterCode
            };

            countryRepository.Add(countryEntity);
            _unitOfWork.Commit();

            string cacheKey = CachePrefix + country.ThreeLetterCode;
            _redisCache.AddAsync(cacheKey, country);

            return Result.Success();
        }

        public Result Delete(string threeLetterCode)
        {
            var countryRepository = _unitOfWork.Repository<Country>();

            var country = countryRepository.Get().SingleOrDefault(c => c.ThreeLetterCode == threeLetterCode);
            if (country == null)
                return Result.Failure([$"Country with code '{threeLetterCode}' not found."]);

            countryRepository.Delete(country.Id);
            _unitOfWork.Commit();

            string cacheKey = CachePrefix + threeLetterCode;
            _redisCache.RemoveAsync(cacheKey);

            return Result.Success();
        }

        public async Task<Result<Country>> Get(string threeLetterCode, CancellationToken cancellationToken = default)
        {
            string cacheKey = CachePrefix + threeLetterCode;

            var cachedCountry = await _redisCache.GetAsync<Country>(cacheKey, cancellationToken);
            if (cachedCountry != null)
                return Result<Country>.Success(cachedCountry, [$"Country '{threeLetterCode}' found in cache."]);

            var countryRepository = _unitOfWork.Repository<Country>();

            var country = countryRepository.Get().FirstOrDefault(c => c.ThreeLetterCode == threeLetterCode);
            if (country != null)
            {
                await _redisCache.AddAsync(cacheKey, country, cancellationToken);
                return Result<Country>.Success(country, [$"Country '{threeLetterCode}' found in database."]);
            }

            return Result<Country>.Failure([$"Country with code '{threeLetterCode}' not found."]);
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
                return Result<PaginatedList<Country>>.Failure(["No countries matched the query."]);

            var paginatedCountries = new PaginatedList<Country>(countries, pagination.Index, pagination.Size);

            return Result<PaginatedList<Country>>.Success(paginatedCountries,
                [$"A total of {paginatedCountries.TotalCount} countries in {paginatedCountries.TotalPages} pages."]);
        }

        public Result Update(CountryInput country)
        {
            var countryRepository = _unitOfWork.Repository<Country>();

            var existingCountry = countryRepository.Get().FirstOrDefault(c => c.ThreeLetterCode == country.ThreeLetterCode);
            if (existingCountry == null)
                return Result.Failure([$"Country with code '{country.ThreeLetterCode}' not found."]);

            existingCountry.Name = country.Name;
            existingCountry.TwoLetterCode = country.TwoLetterCode;
            existingCountry.ThreeLetterCode = country.ThreeLetterCode;

            _unitOfWork.Commit();

            string cacheKey = CachePrefix + country.ThreeLetterCode;
            _redisCache.AddAsync(cacheKey, existingCountry);

            return Result.Success([$"Country with code '{country.ThreeLetterCode}' updated successfully."]);
        }
    }

}
