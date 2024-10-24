using EY.Domain.Contracts;
using EY.Domain.Models;
using EY.Shared.Contracts;
using EY.Shared.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace EY.API.Middlewares
{
    public class TimeoutErrorHandler : IExceptionHandler
    {
        private readonly ILogger<TimeoutErrorHandler> _logger;
        private readonly IJsonHandler _jsonHandler;
        private readonly IUrlHelper _urlHelper;

        public TimeoutErrorHandler(ILogger<TimeoutErrorHandler> logger, IJsonHandler jsonHandler, IUrlHelper urlHelper)
        {
            _logger = logger;
            _jsonHandler = jsonHandler;
            _urlHelper = urlHelper;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is null || !exception.IsAbortedRequestException())
                return false;

            string requestedUri = _urlHelper.GetDisplayUrl(httpContext.Request);
            _logger.LogError(exception, "Requested Url '{RequestedUrl}' has timed out.", requestedUri);

            var result = Result.Failure([ exception.Message ]);
            var json = _jsonHandler.Serialize(result); 

            httpContext.Response.StatusCode = StatusCodes.Status408RequestTimeout;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(json, cancellationToken);

            return true;
        }
    }
}
