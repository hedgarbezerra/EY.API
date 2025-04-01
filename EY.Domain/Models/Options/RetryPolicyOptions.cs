using System.ComponentModel.DataAnnotations;

namespace EY.Domain.Models.Options;

public class RetryPolicyOptions
{
    public const string SettingsKey = "RetryPolicy";
    public const string DEFAULT_PIPELINE = "DefaulyPollyPipeline";

    /// <summary>
    ///     Number of retries that will be made after the first failed run
    /// </summary>
    [Range(0, 5)]
    public required int MaxRetries { get; init; }

    /// <summary>
    ///     Time in seconds between execution attempts
    /// </summary>
    [Range(0, 60)]
    public required int DelayInSeconds { get; init; }

    /// <summary>
    ///     Maximum time in seconds between execution attempts (useful when time is exponential)
    /// </summary>
    [Range(15, 120)]
    public required int MaxDelaySeconds { get; init; }

    /// <summary>
    ///     Execution timeout in seconds
    /// </summary>
    [Range(0, 120)]
    public required int TimeOutSeconds { get; init; }
}