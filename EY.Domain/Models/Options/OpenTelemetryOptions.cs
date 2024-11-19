using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models.Options
{
    public class OpenTelemetryOptions
    {
        public const string SettingsKey = "OpenTelemetry";
        public required string Source { get; init; }
        public JaegerOpenTelemetryOptions Jaeger { get; set; }
        public SeqOpenTelemetryOptions Seq { get; set; }
    }
    public class SeqOpenTelemetryOptions
    {
        public const string SettingsKey = "OpenTelemetry:Seq";

        /// <summary>
        /// Source of logs
        /// </summary>
        /// <summary>
        /// Endpoint to log
        /// </summary>
        public required string Endpoint { get; init; }

        /// <summary>
        /// Authentication key
        /// </summary>
        public required string Key { get; init; }
    }
    public sealed class JaegerOpenTelemetryOptions
    {
        public const string SettingsKey = "OpenTelemetry:Jaeger";
        public required string Endpoint { get; init; }
    }
}
