using EY.Domain.Contracts;
using EY.Domain.Countries;
using EY.Domain.IpAddresses;
using EY.Domain.Models;
using EY.Shared.Attributes;
using OpenTelemetry.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EY.Business.IpAddresses
{
    [BindInterface(typeof(IIpAddressesService))]
    public class IpAddressService : IIpAddressesService
    {
        public const string CachePrefix = "IpAddresses_";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCache _redisCache;
        private readonly IIp2CService _httpConsumer;
        private readonly ISqlExecutor _sqlExecutor;

        public IpAddressService(IUnitOfWork unitOfWork, IRedisCache redisCache, IIp2CService httpConsumer, ISqlExecutor sqlExecutor)
        {
            _unitOfWork = unitOfWork;
            _redisCache = redisCache;
            _httpConsumer = httpConsumer;
            _sqlExecutor = sqlExecutor;
        }

        public Result Add(IpAddressInput ipAddress)
        {
            var ipRepository = _unitOfWork.Repository<IpAddress>();
            var countriesRepository = _unitOfWork.Repository<Country>();

            if (ipRepository.Get().FirstOrDefault(ip => ip.Ip == ipAddress.IpAddress) is not null)
                return Result.Failure([$"IP Address '{ipAddress.IpAddress}' already exists on database."]);

            var country = countriesRepository.Get().FirstOrDefault(c => c.ThreeLetterCode == ipAddress.CountryThreeLetterCode);
            if (country is null)
            {
                country = new Country()
                {
                    Name = ipAddress.CountryName,
                    TwoLetterCode = ipAddress.CountryTwoLetterCode,
                    ThreeLetterCode = ipAddress.CountryThreeLetterCode
                };
                countriesRepository.Add(country);
            }

            var ipEntity = new IpAddress() { Ip = ipAddress.IpAddress, Country = country };
            ipRepository.Add(ipEntity);

            _unitOfWork.Commit();

            string cacheKey = CachePrefix + ipAddress.IpAddress;
            _redisCache.AddAsync(cacheKey, ipEntity);

            return Result.Success();
        }

        public Result Delete(string ipAddress)
        {
            var repository = _unitOfWork.Repository<IpAddress>();

            var ip = repository.Get().SingleOrDefault(i => i.Ip == ipAddress);
            if (ip is null)
                return IpAddressesResults.Errors.IpNotFound(ipAddress);

            repository.Delete(ip.Id);
            _unitOfWork.Commit();

            string cacheKey = CachePrefix + ipAddress;
            _redisCache.RemoveAsync(cacheKey);

            return IpAddressesResults.Deleted(ipAddress);
        }

        public async Task<Result<Ip2CResponse>> Get(string ipAddress, CancellationToken cancellationToken = default)
        {
            string cacheKey = CachePrefix + ipAddress;
            var cachedIp = _redisCache.GetAsync<IpAddress>(cacheKey, CancellationToken.None).Result;
            if (cachedIp is not null)
            {
                var convertedCachedIp = new Ip2CResponse(ipAddress, cachedIp.Country.Name, cachedIp.Country.TwoLetterCode, cachedIp.Country.ThreeLetterCode);
                return IpAddressesResults.FoundCache(convertedCachedIp);
            }

            var repository = _unitOfWork.Repository<IpAddress>();
            var dbIp = repository.Get().FirstOrDefault(ip => ip.Ip == ipAddress);
            if (dbIp is not null)
            {
                await _redisCache.AddAsync(cacheKey, dbIp, cancellationToken);
                var dbConvertedIp = new Ip2CResponse(ipAddress, dbIp.Country.Name, dbIp.Country.TwoLetterCode, dbIp.Country.ThreeLetterCode);
                return IpAddressesResults.FoundDatabase(dbConvertedIp);
            }

            var serviceIpResult = await _httpConsumer.GetIp(ipAddress, CancellationToken.None);
            if (!serviceIpResult.Successful)
                return serviceIpResult;

            var insertionResult = Add(new IpAddressInput(ipAddress, serviceIpResult.Data.CountryName, serviceIpResult.Data.CountryTwoLetterCode, serviceIpResult.Data.CountryThreeLetterCode));

            if (!insertionResult.Successful)
                return Result<Ip2CResponse>.Success(serviceIpResult.Data, $"IP Address found externally, but failed to be saved.");

            dbIp = repository.Get().FirstOrDefault(ip => ip.Ip == ipAddress);

            var ip2CIp = new Ip2CResponse(ipAddress, dbIp.Country.Name, dbIp.Country.TwoLetterCode, dbIp.Country.ThreeLetterCode);

            return IpAddressesResults.FoundExternally(ip2CIp);
        }

        public Result<PaginatedList<Ip2CResponse>> Get(PaginationInput pagination)
        {
            var repository = _unitOfWork.Repository<IpAddress>();
            var ips = repository.Get();

            if (!string.IsNullOrWhiteSpace(pagination.Query))
                ips = ips.Where(ip => ip.Ip.Contains(pagination.Query));

            int totalCount = ips.Count();
            ips = ips.Skip((pagination.Index - 1) * pagination.Size)
               .Take(pagination.Size);

            if (!ips.Any())
                return Result<PaginatedList<Ip2CResponse>>.Failure(["No IP matched the query."]);

            var mappedIps = ips.Select(ip => new Ip2CResponse(ip.Ip, ip.Country.Name, ip.Country.TwoLetterCode, ip.Country.ThreeLetterCode))
                .ToList();
            var paginatedIps = new PaginatedList<Ip2CResponse>(mappedIps, pagination.Index, pagination.Size, totalCount);

            return Result<PaginatedList<Ip2CResponse>>.Success(paginatedIps, [$"A total of {paginatedIps.TotalCount} in {paginatedIps.TotalPages} pages."]);
        }

        public Result<List<IpAddressReportItem>> Report(params string[] twoLetterCountriesCodes)
        {
            var query = _sqlExecutor.Query<IpAddressReportItem>(@"SELECT TOP 100 PERCENT c.Name AS CountryName, COUNT(ips.Id) AS IpAddressesCount, MAX(ips.CreatedAt) AS LastIpAddressUpdatedAt
                                                                    FROM dbo.IPAddresses ips 
                                                                    INNER JOIN dbo.Countries c
                                                                    ON ips.CountryId = c.Id
                                                                    WHERE c.TwoLetterCode in @CountryCodes
                                                                    GROUP BY c.Name
                                                                    ORDER BY COUNT(ips.Id) DESC, MAX(ips.CreatedAt) DESC", new { CountryCodes = twoLetterCountriesCodes });

            return Result<List<IpAddressReportItem>>.Success(query.ToList(), [$"Report returned a total of {query.Count()} rows."]);
        }

        public Result Update(IpAddressInput ipAddress)
        {
            var ipRepository = _unitOfWork.Repository<IpAddress>();
            var countriesRepository = _unitOfWork.Repository<Country>();

            IpAddress ip = ipRepository.Get().FirstOrDefault(ip => ip.Ip == ipAddress.IpAddress);
            if (ip is null)
                return IpAddressesResults.Errors.IpNotFound(ipAddress.IpAddress);

            var country = countriesRepository.Get().FirstOrDefault(c => c.ThreeLetterCode == ipAddress.CountryThreeLetterCode);
            if (country is null)
            {
                country = new Country()
                {
                    Name = ipAddress.CountryName,
                    TwoLetterCode = ipAddress.CountryTwoLetterCode,
                    ThreeLetterCode = ipAddress.CountryThreeLetterCode
                };
                countriesRepository.Add(country);
            }
            ip.Country = country;
            ip.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Commit();

            string cacheKey = CachePrefix + ipAddress;
            _redisCache.AddAsync(cacheKey, ip);

            return IpAddressesResults.Updated(ipAddress.IpAddress);
        }
    }
}
