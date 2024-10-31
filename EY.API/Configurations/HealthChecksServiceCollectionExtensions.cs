using EY.Domain;
using EY.Domain.Models.Options;
using EY.Infrastructure.DataAccess;
using Microsoft.Extensions.Options;

namespace EY.API.Configurations
{
    public static class HealthChecksServiceCollectionExtensions
    {
        public static void AddAPIHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var redisOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
            var otlpOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;
            var connectionString = configuration.GetConnectionString(Constants.ConnectionStrings.SqlServer);

            services.AddHealthChecks()
                .AddSeqPublisher(opt =>
                {
                    opt.Endpoint = otlpOptions.Endpoint;
                    opt.ApiKey = otlpOptions.Key;
                    opt.DefaultInputLevel = HealthChecks.Publisher.Seq.SeqInputLevel.Information;
                })
                .AddRedis(redisOptions.ConnectionString)
                .AddDbContextCheck<AppDbContext>(failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);
        }
    }
}
