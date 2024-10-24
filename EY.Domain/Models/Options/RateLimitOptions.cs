using System;
using System.Collections.Generic;
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
        public required int PermitLimit { get; set; }
        /// <summary>
        /// Time in seconds that the call window is allowed
        /// </summary>
        public required int TimeWindowSeconds { get; set; }
        /// <summary>
        /// Requests in queue awaiting resolution, if 0, will return 503
        /// </summary>
        public required int QueueLimit { get; set; }
    }
}
