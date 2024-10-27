using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EY.API;
using EY.API.Configurations;
using EY.Domain.Models.Options;
using EY.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace EY.IntegrationTests
{
    public class IntegrationsWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly RedisContainer _redisContainer = new RedisBuilder()
            .WithImage("redis:latest")
            .WithPortBinding(6378)
            .WithExposedPort(6378)
            .Build();

        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_PID", "Express")
            .WithExposedPort(1433)
            .Build();

        private readonly IContainer _seqContainer = new ContainerBuilder()
            .WithImage("datalust/seq:latest")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithExposedPort(5341)
            .WithExposedPort(80)
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            var inMemorySettings = new Dictionary<string, string>
                {
                    { "Logging:LogLevel:Default", "Information" },
                    { "Logging:LogLevel:Microsoft.AspNetCore", "Warning" },
                    { "Tasks:IpAddressUpdater:RepeatEveryMinutes", "60" },
                    { "API:Version", "1" },
                    { "Azure:AppConfigurations:ConnectionString", "" },
                    { "Azure:AppConfigurations:CacheSentinel", "1" },
                    { "Azure:AppConfigurations:CacheExpiracySeconds", "120" },
                    { "OpenTelemetry:Endpoint", _seqContainer.Hostname },
                    { "OpenTelemetry:Key", "anykey" },
                    { "OpenTelemetry:Source", "EY.Integration" },
                    { "RateLimit:PermitLimit", "10" },
                    { "RateLimit:TimeWindowSeconds", "60" },
                    { "RateLimit:QueueLimit", "5" },
                    { "Redis:Instance", _redisContainer.Name },
                    { "Redis:ConnectionString", _redisContainer.GetConnectionString() },
                    { "Redis:CacheExpiracyInSeconds", "300" },
                    { "RetryPolicy:MaxRetries", "3" },
                    { "RetryPolicy:DelayInSeconds", "20" },
                    { "RetryPolicy:MaxDelaySeconds", "10" },
                    { "RetryPolicy:TimeOutSeconds", "30" },
                    { "ConnectionStrings:SqlServerInstance", _msSqlContainer.GetConnectionString() },
                    { "AllowedHosts", "*" }
                };
            var configs = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            builder.UseConfiguration(configs);

            builder.ConfigureTestServices(services =>
            {
                //var azureAppConfigDescriptors = services
                //.Where(d => d.ServiceType.Namespace != null && d.ServiceType.Namespace.Contains("Microsoft.Azure.AppConfiguration"))
                //.ToList();

                //foreach (var descriptor in azureAppConfigDescriptors)
                //{
                //    services.Remove(descriptor);
                //}
                //var configsDescriptor = services.SingleOrDefault(p => p.ServiceKey == typeof(IConfiguration));
                //var removeServices = services
                //    .Where(s => s.ServiceType.FullName.Contains("Azure", StringComparison.InvariantCultureIgnoreCase))
                //    .ToList(); 

                //foreach (var service in removeServices)
                //    services.Remove(service);
                // Access configuration after it's been set up
                var sp = services.BuildServiceProvider();

                services.AddSerilogLogging();
                services.AddRedisDistributedCache();

                // Configure the DbContext options
                services.Replace(ServiceDescriptor.Singleton(new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(_msSqlContainer.GetConnectionString())
                    .EnableDetailedErrors()
                    .UseLazyLoadingProxies()
                    .Options));

                // Replace the JsonSerializerSettings
                services.Replace(ServiceDescriptor.Singleton(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                }));

                services.AddAttributeMappedServices();
            });
        }

        public async Task StartContainersAsync()
        {
            await StopContainersAsync();

            await _msSqlContainer.StartAsync();
            await _redisContainer.StartAsync();
            await _seqContainer.StartAsync();
        }

        public async Task StopContainersAsync()
        {
            await _msSqlContainer.StopAsync();
            await _redisContainer.StopAsync();
            await _seqContainer.StopAsync();
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp() => await StartContainersAsync();

        [OneTimeTearDown]
        public async Task OneTimeTearDown() => await StopContainersAsync();
    }
}
