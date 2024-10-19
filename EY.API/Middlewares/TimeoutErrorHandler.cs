using EY.Domain.Models;
using EY.Shared.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace EY.API.Middlewares
{
    public class TimeoutErrorHandler : IExceptionHandler
    {
        private readonly ILogger<TimeoutErrorHandler> _logger;

        public TimeoutErrorHandler(ILogger<TimeoutErrorHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (!exception.IsAbortedRequestException())
                return false;

            string requestedUri = httpContext.Request.GetDisplayUrl();
            _logger.LogError(exception, "Requested Url '{RequestedUrl}' has timed out.", requestedUri);

            var result = Result.Create(false, new List<string> { exception.Message }, new List<string>());
            var json = JsonConvert.SerializeObject(result); 

            httpContext.Response.StatusCode = StatusCodes.Status408RequestTimeout;

            await httpContext.Response.WriteAsync(json, cancellationToken);

            return true;
        }
    }
}
