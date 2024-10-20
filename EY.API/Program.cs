
using EY.API.Middlewares;
using EY.API.Configurations;
using EY.Domain.Models.Options;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using EY.API.BackgroundServices;
using RestSharp;

namespace EY.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //These two lines is optional if appsettings are populated
            builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection(AzureOptions.SettingsKey));
            builder.Services.AddAzureAppConfiguration(builder.Configuration);

            builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection(RateLimitOptions.SettingsKey));
            builder.Services.Configure<RetryPolicyOptions>(builder.Configuration.GetSection(RetryPolicyOptions.SettingsKey));
            builder.Services.Configure<OpenTelemetryOptions>(builder.Configuration.GetSection(OpenTelemetryOptions.SettingsKey));
            builder.Services.Configure<RedisCacheOptions>(builder.Configuration.GetSection(RedisCacheOptions.SettingsKey));

            builder.Services.AddOtlpLogging();
            var logger = builder.Services.BuildServiceProvider()?.GetRequiredService<ILogger<Program>>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<RestClient>();
            builder.Services.AddAttributeMappedServices();
            builder.Services.AddAPIVersioning(builder.Configuration);            
            builder.Services.AddRedisDistributedCache();
            builder.Services.AddAPIResiliencePipeline(logger);
            builder.Services.AddAPIRateLimiter();
            builder.Services.AddEntityFramework(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddExceptionHandler<TimeoutErrorHandler>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddHostedService<IpAddressUpdaterTask>();
            var app = builder.Build();
            logger.LogInformation("Done");

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "openapi/{documentName}.json";
            });
            app.MapScalarApiReference(options =>
            {
                options.ForceThemeMode = ThemeMode.Dark;
                options.Theme = ScalarTheme.Purple;
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseExceptionHandler();
            app.UseAzureAppConfiguration();
            app.UseCors(opt =>
            {
                opt.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            });
            app.UseMiddleware<SimpleAuthenticationMiddleware>();

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}
