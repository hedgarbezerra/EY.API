using EY.API.Middlewares;
using EY.API.Configurations;
using EY.Domain.Models.Options;
using Scalar.AspNetCore;
using EY.API.BackgroundServices;
using RestSharp;
using Serilog;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

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
            builder.Services.Configure<RedisCacheOptions>(builder.Configuration.GetSection(RedisCacheOptions.SettingsKey));

            builder.Services.AddSerilogLogging();
            var logger = builder.Services.BuildServiceProvider()?.GetRequiredService<ILogger<Program>>();

            builder.Services.AddScoped<IRestClient, RestClient>();
            builder.Services.AddSingleton(_ => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Error = (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) 
                            => logger?.LogError(args.ErrorContext.Error,
                                "An error occurred while serializing object type '{ObjectType}'. Reason: {ErrorReason}",
                                args.ErrorContext.OriginalObject.GetType().FullName,
                                args.ErrorContext.Error.Message),
            });

            builder.Services.AddHttpContextAccessor();
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

            if (builder.Environment.IsProduction() || builder.Environment.IsDevelopment())
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

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseExceptionHandler();
            app.UseCors(opt =>
            {
                opt.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
            });
            app.UseMiddleware<SimpleAuthenticationMiddleware>();
            app.UseRateLimiter();
            app.UseHealthChecks("_health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapControllers();

            app.Run();
        }
    }
}
