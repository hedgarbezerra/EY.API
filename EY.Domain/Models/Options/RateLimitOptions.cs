using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models.Options
{
    public class RateLimitOptions
    {
        public const string SettingsKey = "RateLimit";
        public const string DEFAULT_POLICY = "RateLimitDefaultPolicy";

        /// <summary>
        /// Maximum calls per window 
        /// </summary>
        [Range(1, 10)]
        public required int PermitLimit { get; init; }
        /// <summary>
        /// Time in seconds that the call window is allowed
        /// </summary>
        [Range(15, 120)]
        public required int TimeWindowSeconds { get; init; }
        /// <summary>
        /// Requests in queue awaiting resolution, if 0, will return 503
        /// </summary>
        [Range(0, 10)]
        public required int QueueLimit { get; init; }
    }
}
