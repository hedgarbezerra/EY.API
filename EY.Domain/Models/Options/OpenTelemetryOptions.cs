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

        /// <summary>
        /// Source of logs
        /// </summary>
        public required string Source { get; set; }
        /// <summary>
        /// Endpoint to log
        /// </summary>
        public required string Endpoint { get; set; }

        /// <summary>
        /// Authentication key
        /// </summary>
        public required string Key { get; set; }
    }
}
