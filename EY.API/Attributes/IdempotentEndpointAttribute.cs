using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace EY.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class IdempotentEndpointAttribute : Attribute, IAsyncActionFilter
    {
        private const int DefaultCacheTimeInMinutes = 10;
        private const string HttpRequestIdempotencyHeaderName = "Idempotence-Key";
        private const string CacheIdempotencyPrefix = "Idempotent_";

        private readonly TimeSpan _cacheDuration;

        public IdempotentEndpointAttribute(int cacheTimeInMinutes = DefaultCacheTimeInMinutes)
        {
            _cacheDuration = TimeSpan.FromMinutes(cacheTimeInMinutes);
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // Parse the Idempotence-Key header from the request
            if (!context.HttpContext.Request.Headers.TryGetValue(HttpRequestIdempotencyHeaderName, out StringValues idempotenceKeyValue) ||
                !Guid.TryParse(idempotenceKeyValue, out Guid idempotenceKey))
            {
                context.Result = new BadRequestObjectResult("Invalid or missing Idempotence-Key header");
                return;
            }

            IDistributedCache cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

            // Check if we already processed this request and return a cached response (if it exists)
            string cacheKey = $"{CacheIdempotencyPrefix}{idempotenceKey}";
            string? cachedResult = await cache.GetStringAsync(cacheKey);
            if (cachedResult is not null)
            {
                IdempotentResponse response = JsonConvert.DeserializeObject<IdempotentResponse>(cachedResult)!;

                var result = new ObjectResult(response.Value) { StatusCode = response.StatusCode };
                context.Result = result;

                return;
            }

            // Execute the request and cache the response for the specified duration
            ActionExecutedContext executedContext = await next();

            if (executedContext.Result is ObjectResult { StatusCode: >= 200 and < 300 } objectResult)
            {
                int statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
                IdempotentResponse response = new(statusCode, objectResult.Value);

                await cache.SetStringAsync(
                    cacheKey,
                    JsonConvert.SerializeObject(response),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration }
                );
            }
        }
    }
    file sealed class IdempotentResponse
    {
        [JsonConstructor]
        public IdempotentResponse(int statusCode, object? value)
        {
            StatusCode = statusCode;
            Value = value;
        }

        public int StatusCode { get; }
        public object? Value { get; }
    }
}