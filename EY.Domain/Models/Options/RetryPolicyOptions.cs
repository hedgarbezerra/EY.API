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
        public required int MaxRetries { get; init; }

        /// <summary>
        /// Time in seconds between execution attempts 
        /// </summary>
        public required int DelayInSeconds { get; init; }
        /// <summary>
        /// Maximum time in seconds between execution attempts (useful when time is exponential)
        /// </summary>
        public required int MaxDelaySeconds { get; init; }
        /// <summary>
        /// Execution timeout in seconds
        /// </summary>
        public required int TimeOutSeconds { get; init; }
    }
}
