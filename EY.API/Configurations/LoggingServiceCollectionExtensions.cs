using EY.Domain.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;

namespace EY.API.Configurations
{
    public static class LoggingServiceCollectionExtensions
    {

        /// <summary>
        /// Enables loggin, tracing and metrics with Serilog OpenTelemetry Provider
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <param name="logger">Application logger builder to add new logger</param>
        /// <returns>Collection of services</returns>
        /// <exception cref="ApplicationException">In the case of appsettings not being configured</exception>
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
                 .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose)
                 .WriteTo.OpenTelemetry(opt =>
                 {
                     opt.Endpoint = otlpOptions.Endpoint;
                     opt.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.HttpProtobuf;
                     opt.Headers = new Dictionary<string, string>()
                     {
                         ["X-Seq-ApiKey"] = otlpOptions.Key
                     };
                     opt.ResourceAttributes = new Dictionary<string, object>()
                     {
                         ["service.name"] = otlpOptions.Source
                     };
                 })
                 .CreateLogger();

            services.AddSerilog();

            return services;
        }


        /// <summary>
        /// Enables logging, tracing and metrics with OpenTelemetry provider
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <param name="logger">Application logger builder to add new logger</param>
        /// <returns>Collection of services</returns>
        /// <exception cref="ApplicationException">In the case of appsettings not being configured</exception>
        public static IServiceCollection AddOtlpLogging(this IServiceCollection services)
        {
            var otlpOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;
            var environment = services.BuildServiceProvider()?.GetRequiredService<IWebHostEnvironment>();

            services.AddOpenTelemetry()
                .ConfigureResource(r =>
                {
                    r.AddService(otlpOptions.Source);
                    r.AddAttributes(new Dictionary<string, object>
                    {
                        ["Environment"] = environment?.EnvironmentName ?? string.Empty,
                    });
                })
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(options => ConfigureOpenTelemetryFunction(options, otlpOptions)))
                .WithLogging(logging => logging
                    .AddConsoleExporter()
                    .AddOtlpExporter(options => ConfigureOpenTelemetryFunction(options, otlpOptions)));

            services.AddLogging(builder =>
               builder.AddOpenTelemetry(options =>
               {
                   options.IncludeFormattedMessage = true;
                   options.IncludeScopes = true;
                   options.AddConsoleExporter();
                   options.AddOtlpExporter(exporter =>
                   {
                       exporter.Endpoint = new Uri(otlpOptions.Endpoint);
                       exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
                       exporter.Headers = $"X-Seq-ApiKey={otlpOptions.Key}";
                   });
               }));
            return services;
        }

        private static Action<OtlpExporterOptions, OpenTelemetryOptions> ConfigureOpenTelemetryFunction = (options, otlpOptions) =>
        {
            options.Endpoint = new Uri(otlpOptions.Endpoint);
            options.Headers = $"X-Seq-ApiKey={otlpOptions.Key}";
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
        };
    }
}
