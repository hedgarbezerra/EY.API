using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models.Options
{
    public class OpenTelemetryOptions
    {
        public const string SettingsKey = "OpenTelemetry";
        [Required(AllowEmptyStrings = false)]
        public required string Source { get; init; }
        public required JaegerOpenTelemetryOptions Jaeger { get; set; }
        public required SeqOpenTelemetryOptions Seq { get; set; }
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
        [Required(AllowEmptyStrings = false)]
        public required string Endpoint { get; init; }

        /// <summary>
        /// Authentication key
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public required string Key { get; init; }
    }
    public sealed class JaegerOpenTelemetryOptions
    {
        public const string SettingsKey = "OpenTelemetry:Jaeger";
        [Required(AllowEmptyStrings = false)]
        public required string Endpoint { get; init; }
    }
}
