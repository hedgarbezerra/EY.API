using Microsoft.Extensions.Http.Diagnostics;
using Polly;

namespace EY.Shared.Extensions;

public static class PollyExtensions
{
    private static readonly ResiliencePropertyKey<RequestMetadata?> _requestMetadataKey =
        new("Extensions-RequestMetadata");

    public static string GetContextURL(this ResilienceContext context)
    {
        var requestMetadata = GetContextMetadata(context);

        return requestMetadata?.RequestRoute ?? string.Empty;
    }

    public static RequestMetadata GetContextMetadata(this ResilienceContext context)
    {
        return context.Properties.GetValue(_requestMetadataKey, null);
    }
}