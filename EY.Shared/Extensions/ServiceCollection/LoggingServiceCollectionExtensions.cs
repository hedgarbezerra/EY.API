using EY.Domain.Models.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenTelemetry;

namespace EY.Shared.Extensions.ServiceCollection;

public static class LoggingServiceCollectionExtensions
{
    /// <summary>
    ///     Enables loggin, tracing and metrics with Serilog OpenTelemetry Provider
    /// </summary>
    /// <param name="services">Collection of services on DI container</param>
    /// <returns>Collection of services</returns>
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services)
    {
        var otlpOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;
        var environment = services.BuildServiceProvider()?.GetRequiredService<IWebHostEnvironment>();
        if (otlpOptions is null || environment is null)
            return services;

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.OpenTelemetry(opt =>
            {
                opt.Endpoint = otlpOptions.Seq.Endpoint;
                opt.Protocol = OtlpProtocol.HttpProtobuf;
                opt.Headers = new Dictionary<string, string>
                {
                    ["X-Seq-ApiKey"] = otlpOptions.Seq.Key
                };
                opt.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = otlpOptions.Source
                };
            })
            .Filter.ByExcluding(evt => 
                evt.Properties.ContainsKey("RequestPath") && 
                (evt.Properties["RequestPath"].ToString().Contains("metrics") ||
                evt.Properties["RequestPath"].ToString().Contains("health")))
            .CreateLogger();

        services.AddSerilog();

        return services;
    }


    /// <summary>
    ///     Enables logging, tracing and metrics with OpenTelemetry provider
    /// </summary>
    /// <param name="services">Collection of services on DI container</param>
    /// <returns>Collection of services</returns>
    public static IServiceCollection AddStructuredLogging(this IServiceCollection services)
    {
        var otlpOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;
        var environment = services.BuildServiceProvider()?.GetRequiredService<IWebHostEnvironment>();

        Action<OtlpExporterOptions> configureAction = options =>
        {
            options.Endpoint = new Uri(otlpOptions.Seq.Endpoint);
            options.Headers = $"X-Seq-ApiKey={otlpOptions.Seq.Key}";
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
        };
        ConfigureOpenTelemetry(services, environment.EnvironmentName, otlpOptions.Source, configureAction);

        return services;
    }


    /// <summary>
    ///     Enables tracing and metrics with OpenTelemetry provider
    /// </summary>
    /// <param name="services">Collection of services on DI container</param>
    /// <returns>Collection of services</returns>
    public static IServiceCollection AddDistributedTracing(this IServiceCollection services)
    {
        var otlpOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;
        var environment = services.BuildServiceProvider()?.GetRequiredService<IWebHostEnvironment>();

        ConfigureOpenTelemetry(services, environment.EnvironmentName, otlpOptions.Source,
            options => { options.Endpoint = new Uri(otlpOptions.Jaeger.Endpoint); });
        return services;
    }


    /// <summary>
    ///     Enables logging, tracing and metrics with OpenTelemetry providers
    /// </summary>
    /// <param name="services">Collection of services on DI container</param>
    /// <returns>Collection of services</returns>
    public static IServiceCollection AddDistributedOpenTelemetry(this IServiceCollection services)
    {
        var otlpOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;
        var environment = services.BuildServiceProvider()?.GetRequiredService<IWebHostEnvironment>();

        services.AddSerilogLogging();

        services.AddOpenTelemetry()
            .ConfigureResource(r =>
            {
                r.AddService(otlpOptions.Source);
                r.AddAttributes(new Dictionary<string, object>
                {
                    ["Environment"] = environment.EnvironmentName
                });
            })
            .WithTracing(tracing => tracing 
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = context => 
                        !context.Request.Path.StartsWithSegments("/_health") && 
                        !context.Request.Path.StartsWithSegments("/_metrics");
                })
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddRedisInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(options => { options.Endpoint = new Uri(otlpOptions.Jaeger.Endpoint); }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter(opt =>
                {
                    opt.ScrapeEndpointPath = "/_metrics";
                })
                .AddView("http.server.request.duration",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = new[] { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
                    })
            );
        //.WithLogging(logging => logging
        //    .AddConsoleExporter()
        //    .AddOtlpExporter(options =>
        //    {
        //        options.Endpoint = new Uri(otlpOptions.Seq.Endpoint);
        //        options.Headers = $"X-Seq-ApiKey={otlpOptions.Seq.Key}";
        //        options.Protocol = OtlpExportProtocol.HttpProtobuf;
        //    }));

        return services;
    }

    private static void ConfigureOpenTelemetry(IServiceCollection services, string environmentName, string source,
        Action<OtlpExporterOptions> configureAction = null)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r =>
            {
                r.AddService(source);
                r.AddAttributes(new Dictionary<string, object>
                {
                    ["Environment"] = environmentName
                });
            })
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddRedisInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(options => configureAction?.Invoke(options)))
            .WithMetrics(metrics =>
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter()
                    .AddOtlpExporter()
            )
            .WithLogging(logging => logging
                .AddConsoleExporter()
                .AddOtlpExporter(options => configureAction?.Invoke(options)));
    }
}