﻿using System.Diagnostics;
using EY.Business.IpAddresses;
using EY.Domain;
using EY.Domain.Contracts;
using EY.Domain.Countries;
using EY.Domain.IpAddresses;

namespace EY.API.BackgroundServices;

public class IpAddressUpdaterTask : BackgroundService
{
    private const int BatchSize = 100;

    private readonly int _repeatEveryMinutes;
    private readonly IServiceScopeFactory _scopeFactory;

    public IpAddressUpdaterTask(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _repeatEveryMinutes = configuration.GetValue<int>(Constants.Options.Tasks.IpAddressUpdater_RepeatInMinutesKey);
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_repeatEveryMinutes));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            using (var scope = _scopeFactory.CreateScope())
            {
                var stopwatch = Stopwatch.StartNew();
                ILogger<IpAddressUpdaterTask> logger =
                    scope.ServiceProvider.GetRequiredService<ILogger<IpAddressUpdaterTask>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var ip2CService = scope.ServiceProvider.GetRequiredService<IIp2CService>();
                var redisCache = scope.ServiceProvider.GetRequiredService<IRedisCache>();

                try
                {
                    logger.LogInformation("Running IP Addresses update task at {ExecutionStartTime}",
                        DateTimeOffset.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    List<string> ipAddressesToRemoveCache = [];

                    var ipsRepository = unitOfWork.Repository<IpAddress>();
                    var countriesRepository = unitOfWork.Repository<Country>();
                    var bulkRepo = ipsRepository as IRepositoryBulk<IpAddress>;

                    var ipAddresses = ipsRepository.Get();
                    var executedCount = 0;
                    while (executedCount < ipAddresses.Count())
                    {
                        var batch = ipAddresses.Skip(executedCount).Take(BatchSize).ToList();
                        List<IpAddress> updatedIpAddresses =
                            await BatchUpdateIpAddresses(ip2CService, countriesRepository, batch, stoppingToken);

                        bulkRepo.BulkUpdate(updatedIpAddresses.ToArray());
                        executedCount += BatchSize;
                        ipAddressesToRemoveCache.AddRange(updatedIpAddresses.Select(ip => ip.Ip));
                    }

                    unitOfWork.Commit();
                    UpdateCache(redisCache, ipAddressesToRemoveCache);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
                finally
                {
                    stopwatch.Stop();
                    logger.LogInformation(
                        "IP Addresses update task runned for {ExecutiongTimeInMs}ms and finished at {ExecutionFinishTime}",
                        stopwatch.ElapsedMilliseconds, DateTimeOffset.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                }
            }
    }

    private static async Task<List<IpAddress>> BatchUpdateIpAddresses(IIp2CService ip2CService,
        IRepository<Country> countriesRepository, List<IpAddress> batch, CancellationToken stoppingToken)
    {
        var updateFromIp2CTasks = batch.Select(ip => ip2CService.GetIp(ip.Ip, stoppingToken));

        var ip2CResults = await Task.WhenAll(updateFromIp2CTasks);
        var updatedIpAddresses = new List<IpAddress>();

        foreach (var ip2CResult in ip2CResults)
        {
            if (ip2CResult.Data is null)
                continue;

            var ipAddressEntity = batch.FirstOrDefault(ip => ip.Ip == ip2CResult.Data.IpAddress);
            var isIpUpdated = ip2CResult.Data.IsIpAddressUpdated(ipAddressEntity);

            if (ipAddressEntity is not null && isIpUpdated)
            {
                var country = countriesRepository.Get()
                    .FirstOrDefault(c => c.ThreeLetterCode == ip2CResult.Data.CountryThreeLetterCode);
                if (country is null)
                {
                    country = new Country
                    {
                        Name = ip2CResult.Data.CountryName,
                        TwoLetterCode = ip2CResult.Data.CountryTwoLetterCode,
                        ThreeLetterCode = ip2CResult.Data.CountryThreeLetterCode
                    };
                    countriesRepository.Add(country);
                }

                ipAddressEntity.Country = country;
                updatedIpAddresses.Add(ipAddressEntity);
            }
        }

        return updatedIpAddresses;
    }

    private Task UpdateCache(IRedisCache cache, List<string> ipAddresses)
    {
        var updateCacheTasks = ipAddresses.Select(ip => cache.RemoveAsync($"{IpAddressesService.CachePrefix}{ip}"));

        return Task.WhenAll(updateCacheTasks);
    }
}