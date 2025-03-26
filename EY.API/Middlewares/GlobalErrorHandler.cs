using EY.Domain.Contracts;
using EY.Domain.Models;
using EY.Shared.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace EY.API.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger,
        Shared.Contracts.IUrlHelper _urlHelper,
        IProblemDetailsService _problemDetailsService) : IExceptionHandler
    {


        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is null)
                return false;

            string requestedUri = _urlHelper.GetDisplayUrl(httpContext.Request);
            _logger.LogError(exception, "Exception occurred at {RequestedUrl}: {Message} and was caught by global handler.", requestedUri, exception.Message);


            var extensions = new Dictionary<string, object>()
            {
                ["Trace"] = httpContext.TraceIdentifier,
                ["Endpoint"] = requestedUri,

            };

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred while processing your request.",
                    Detail = exception.Message,
                    Instance = requestedUri,
                    Extensions = extensions,
                },
            });
        }
    }
}
