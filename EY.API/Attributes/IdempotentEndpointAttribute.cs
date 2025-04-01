using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EY.API.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
internal sealed class IdempotentEndpointAttribute : Attribute, IAsyncActionFilter
{
    private const int DefaultCacheTimeInMinutes = 10;
    private const string HttpRequestIdempotencyHeaderName = "Idempotence-Key";
    private const string CacheIdempotencyPrefix = "Idempotent_";

    private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;

    public IdempotentEndpointAttribute(int cacheTimeInMinutes = DefaultCacheTimeInMinutes)
    {
        var cacheDuration = TimeSpan.FromMinutes(cacheTimeInMinutes);
        _distributedCacheEntryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cacheDuration };
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var problemDetailsService = context.HttpContext.RequestServices
            .GetRequiredService<IProblemDetailsService>();
        // Parse the Idempotence-Key header from the request
        if (!context.HttpContext.Request.Headers.TryGetValue(HttpRequestIdempotencyHeaderName,
                out var idempotenceKeyValue) ||
            !Guid.TryParse(idempotenceKeyValue, out var idempotenceKey))
        {
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context.HttpContext,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Idempotency Header",
                    Detail = "The 'Idempotence-Key' header is missing or has an invalid format. Please provide a valid GUID.",
                }
            }); 
            return;
        }

        var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();

        // Check if we already processed this request and return a cached response (if it exists)
        var cacheKey = $"{CacheIdempotencyPrefix}{idempotenceKey}";
        var cachedResult = await cache.GetStringAsync(cacheKey);
        if (cachedResult is not null)
        {
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context.HttpContext,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Request In Progress",
                    Detail = "A request with this idempotency key is currently being processed. Please try again later.",
                }
            });
            var result = new ObjectResult(null) { StatusCode = StatusCodes.Status409Conflict };
            context.Result = result;

            return;
        }

        await cache.SetStringAsync(
            cacheKey,
            "",
            _distributedCacheEntryOptions
        );
        
        // Execute the request and cache the response for the specified duration
        var executedContext = await next();

        if (executedContext.Result is ObjectResult { StatusCode: >= 200 and < 500 })
        {
            cache.RemoveAsync(cacheKey);
        }
    }
}