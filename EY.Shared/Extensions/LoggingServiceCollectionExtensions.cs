using EY.Domain.Models.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Shared.Extensions
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection services, ILoggingBuilder logger)
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
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter(options => ConfigureOpenTelemetryFunction(options, otlpOptions)))
                .WithLogging(logging => logging
                    .AddConsoleExporter()
                    .AddOtlpExporter(options => ConfigureOpenTelemetryFunction(options, otlpOptions)));

            logger.AddOpenTelemetry(logging =>
            {
                logging.IncludeScopes = true;
                logging.IncludeFormattedMessage = true;
                logging.AddOtlpExporter(options => ConfigureOpenTelemetryFunction(options, otlpOptions));
            });

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
