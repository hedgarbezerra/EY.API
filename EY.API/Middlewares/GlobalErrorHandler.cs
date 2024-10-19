using EY.Domain.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace EY.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string requestedUri = httpContext.Request.GetDisplayUrl();
            _logger.LogError(exception, "Exception occurred at {RequestedUrl}: {Message} and was caught by global handler.", requestedUri, exception.Message);

            var result = Result.Create(false, new List<string> { exception.Message }, new List<string>());
            var jsonResult = JsonConvert.SerializeObject(result);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsync(jsonResult, cancellationToken);

            return true;
        }
    }
}
