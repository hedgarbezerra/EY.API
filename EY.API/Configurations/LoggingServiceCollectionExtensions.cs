using EY.Domain.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EY.API.Configurations
{
    public static class LoggingServiceCollectionExtensions
    {
        /// <summary>
        /// Enables connection to OpenTelemetry providers
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <param name="logger">Application logger builder to add new logger</param>
        /// <returns>Collection of services</returns>
        /// <exception cref="ApplicationException">In the case of appsettings not being configured</exception>
        public static IServiceCollection AddOtlpLogging(this IServiceCollection services)
        {
            var otlpOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<OpenTelemetryOptions>>().Value;
            var environment = services.BuildServiceProvider()?.GetRequiredService<IWebHostEnvironment>();

            if (otlpOptions is null)
                throw new ApplicationException("Unable to load Open Telemtry options.");

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
