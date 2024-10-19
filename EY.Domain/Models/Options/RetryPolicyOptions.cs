using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models.Options
{
    public class RetryPolicyOptions
    {
        public const string SettingsKey = "RetryPolicy";
        public const string DEFAULT_PIPELINE = "DefaulyPollyPipeline";

        /// <summary>
        /// Number of retries that will be made after the first failed run
        /// </summary>
        public required int MaxRetries { get; set; }

        /// <summary>
        /// Time in seconds between execution attempts 
        /// </summary>
        public required int DelayInSeconds { get; set; }
        /// <summary>
        /// Maximum time in seconds between execution attempts (useful when time is exponential)
        /// </summary>
        public required int MaxDelaySeconds { get; set; }
        /// <summary>
        /// Execution timeout in seconds
        /// </summary>
        public required int TimeOutSeconds { get; set; }
    }
}
