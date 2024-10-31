using EY.Domain.Contracts;
using EY.Domain.Models;
using EY.Shared.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using RestSharp;

namespace EY.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IJsonHandler _jsonHandler;
        private readonly IUrlHelper _urlHelper;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IJsonHandler jsonHandler, IUrlHelper urlHelper)
        {
            _logger = logger;
            _jsonHandler = jsonHandler;
            _urlHelper = urlHelper;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is null)
                return false;

            string requestedUri = _urlHelper.GetDisplayUrl(httpContext.Request);
            _logger.LogError(exception, "Exception occurred at {RequestedUrl}: {Message} and was caught by global handler.", requestedUri, exception.Message);


            var extensions = new Dictionary<string, object>()
            {
                ["Trace"] = httpContext.TraceIdentifier
            };
            var problems = Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: exception.Message,
                title: "An error occurred while processing your request.",
                extensions: extensions);

            await problems.ExecuteAsync(httpContext);
            //var result = Result.Failure([ exception.Message ]);
            //var jsonResult = _jsonHandler.Serialize(result);

            //httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //httpContext.Response.ContentType = "application/json";

            //await httpContext.Response.WriteAsync(jsonResult, cancellationToken);
            return true;
        }
    }
}
