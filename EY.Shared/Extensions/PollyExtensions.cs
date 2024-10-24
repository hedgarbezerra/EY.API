using Polly;
using Microsoft.Extensions.Http.Diagnostics;

namespace EY.Shared.Extensions
{
    public static class PollyExtensions
    {
        private static readonly ResiliencePropertyKey<RequestMetadata?> _requestMetadataKey = new ResiliencePropertyKey<RequestMetadata>("Extensions-RequestMetadata");
        public static string GetContextURL(this ResilienceContext context)
        {
            var requestMetadata = GetContextMetadata(context);

            return requestMetadata?.RequestRoute ?? string.Empty;
        }

        public static RequestMetadata GetContextMetadata(this ResilienceContext context) => context.Properties.GetValue(_requestMetadataKey, null);
    }
}
