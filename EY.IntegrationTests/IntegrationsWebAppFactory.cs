using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EY.API;
using EY.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace EY.IntegrationTests;

public class IntegrationsWebAppFactory : WebApplicationFactory<Program>
{
    private readonly IContainer _jaegerContainer = new ContainerBuilder()
        .WithImage("jaegertracing/all-in-one:latest")
        .WithPortBinding(4318, true)
        .WithPortBinding(16686, true)
        .Build();

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("MSSQL_PID", "Express")
        .WithPortBinding(1433, true)
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .WithPortBinding(6378, true)
        .Build();

    private readonly IContainer _seqContainer = new ContainerBuilder()
        .WithImage("datalust/seq:latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithPortBinding(5341, true)
        .WithPortBinding(80, true)
        .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        var inMemorySettings = new Dictionary<string, string>
        {
            { "OpenTelemetry:Seq:Endpoint", _seqContainer.Hostname },
            { "OpenTelemetry:Jaeger:Endpoint", $"http://{_jaegerContainer.Hostname}:{4318}" },
            { "Redis:Instance", _redisContainer.Name },
            { "Redis:ConnectionString", _redisContainer.GetConnectionString() },
            { "ConnectionStrings:SqlServerInstance", _msSqlContainer.GetConnectionString() }
        };
        var configs = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json", false, true)
            .AddInMemoryCollection(inMemorySettings).Build();
        builder.UseConfiguration(configs);

        builder.ConfigureTestServices(services =>
        {
            // Configure the DbContext options
            services.Replace(ServiceDescriptor.Singleton(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_msSqlContainer.GetConnectionString())
                .EnableDetailedErrors()
                .Options));
        });
    }

    public async Task StartContainersAsync()
    {
        await StopContainersAsync();

        await _msSqlContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _seqContainer.StartAsync();
        await _jaegerContainer.StartAsync();
    }

    public async Task StopContainersAsync()
    {
        await _msSqlContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _seqContainer.StopAsync();
        await _jaegerContainer.StopAsync();
    }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await StartContainersAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await StopContainersAsync();
    }
}