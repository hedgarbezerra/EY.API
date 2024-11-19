using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace EY.API.Middlewares
{
    public class RateLimitHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //if (context.Features.Get<RateLimiterOptions>() is { } rateLimitOptions)
            //{
            //    var x = await rateLimitOptions.GlobalLimiter.AcquireAsync(context);
            //    x.
            //    var headers = context.Response.Headers;
            //    headers["X-RateLimit-Limit"] = rateLimitFeature.GlobalLimiter.Limiter.PermitLimit.ToString();
            //    headers["X-RateLimit-Remaining"] = rateLimitFeature.Limiter.TryReplenish().ToString();
            //    headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.Add(rateLimitFeature.Limiter.Window).ToString("o");
            //}
            //// Tenta obter a política "FixedWindowPolicy" do rate limiter
            //if (_rateLimiter..TryGetPolicy("FixedWindowPolicy", out var policy) &&
            //    policy is FixedWindowRateLimiter fixedWindowLimiter)
            //{
            //    // Verifica o estado do limite para a requisição atual
            //    var lease = await fixedWindowLimiter.AttemptLeaseAsync(1);

            //    // Adiciona cabeçalhos de limite de taxa
            //    context.Response.OnStarting(() =>
            //    {
            //        context.Response.Headers["X-RateLimit-Limit"] = fixedWindowLimiter.PermitLimit.ToString();
            //        context.Response.Headers["X-RateLimit-Remaining"] = lease.IsAcquired ? "1" : "0";
            //        context.Response.Headers["X-RateLimit-Reset"] = DateTime.UtcNow.Add(fixedWindowLimiter.Window).ToString("o");

            //        return Task.CompletedTask;
            //    });
            //}

            await _next(context);
        }
    }
}
