using EY.API.Middlewares;
using EY.API.Configurations;
using EY.Domain.Models.Options;
using Scalar.AspNetCore;
using EY.API.BackgroundServices;
using RestSharp;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using EY.Infrastructure.Contracts;
using EY.Shared.Extensions.ServiceCollection;

namespace EY.API
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection(AzureOptions.SettingsKey));
            builder.Services.AddAzureAppConfiguration(builder.Configuration);

            builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection(RateLimitOptions.SettingsKey));
            builder.Services.Configure<RetryPolicyOptions>(builder.Configuration.GetSection(RetryPolicyOptions.SettingsKey));
            builder.Services.Configure<OpenTelemetryOptions>(builder.Configuration.GetSection(OpenTelemetryOptions.SettingsKey));
            builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(AuthenticationOptions.SettingsKey));
            builder.Services.Configure<RedisCacheOptions>(builder.Configuration.GetSection(RedisCacheOptions.SettingsKey));

            var logger = builder.Services.BuildServiceProvider()?.GetRequiredService<ILogger<Program>>();

            builder.Services.AddScoped<IRestClient, RestClient>(_ => new RestClient());

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedOpenTelemetry();
            builder.Services.AddAPIMappedServices();
            builder.Services.AddAPIVersioning(builder.Configuration);            
            builder.Services.AddAPIAuthentication();            
            builder.Services.AddRedisDistributedCache();
            builder.Services.AddAPIResiliencePipeline(logger);
            builder.Services.AddAPIRateLimiter();
            builder.Services.AddEntityFramework(builder.Configuration);
            builder.Services.AddAPIHealthChecks(builder.Configuration);
            builder.Services.AddControllers().AddNewtonsoftJson();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddExceptionHandler<TimeoutErrorHandler>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddHostedService<IpAddressUpdaterTask>();
            var app = builder.Build();

            if (builder.Environment.IsProduction())
            {
                app.UseAzureAppConfiguration();
            }

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "openapi/{documentName}.json";
            });
            app.MapScalarApiReference(options =>
            {
                options.ForceThemeMode = ThemeMode.Dark;
                options.Theme = ScalarTheme.Purple;
            });

            app.UseExceptionHandler();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseCors(opt =>
            {
                opt.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            });
            app.UseRateLimiter();
            app.UseOutputCache();
            app.UseHealthChecks("/_health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
            app.MapControllers();
            app.UseMiddleware<ActivityEnrichmentMiddleware>();


            using (var scope = app.Services.CreateScope())
            {
                var migrator = scope.ServiceProvider.GetRequiredService<IMigrationsExecuter>();
                migrator.Migrate();
            }

            app.Run();
        }
    }
}
