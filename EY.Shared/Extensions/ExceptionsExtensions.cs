using Polly.Timeout;

namespace EY.Shared.Extensions;

public static class ExceptionsExtensions
{
    public static bool IsAbortedRequestException(this Exception exception)
    {
        if (exception is HttpRequestException)
        {
            var httpExpection = exception as HttpRequestException;
            if (httpExpection?.InnerException is not TaskCanceledException)
                return false;

            return true;
        }

        if (exception is TimeoutRejectedException)
            return true;

        return false;
    }
}