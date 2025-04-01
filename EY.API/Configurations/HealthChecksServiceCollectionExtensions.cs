using EY.Domain;
using EY.Domain.Models.Options;
using EY.Infrastructure.DataAccess;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace EY.API.Configurations;

public static class HealthChecksServiceCollectionExtensions
{
    public static void AddAPIHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
        var otlpOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<SeqOpenTelemetryOptions>>()
            .Value;
        var connectionString = configuration.GetConnectionString(Constants.ConnectionStrings.SqlServer);

        services.AddHealthChecks()
            .AddRedis(redisOptions.ConnectionString)
            .AddDbContextCheck<AppDbContext>(failureStatus: HealthStatus.Unhealthy);
    }
}