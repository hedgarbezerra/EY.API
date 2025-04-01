using EY.Domain.Contracts;
using EY.Shared.Contracts;
using EY.Shared.Extensions;
using Microsoft.AspNetCore.Diagnostics;

namespace EY.API.Middlewares;

public class TimeoutExceptionHandler : IExceptionHandler
{
    private readonly IJsonHandler _jsonHandler;
    private readonly ILogger<TimeoutExceptionHandler> _logger;
    private readonly IUrlHelper _urlHelper;

    public TimeoutExceptionHandler(ILogger<TimeoutExceptionHandler> logger, IJsonHandler jsonHandler,
        IUrlHelper urlHelper)
    {
        _logger = logger;
        _jsonHandler = jsonHandler;
        _urlHelper = urlHelper;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is null || !exception.IsAbortedRequestException())
            return false;

        var requestedUri = _urlHelper.GetDisplayUrl(httpContext.Request);
        _logger.LogError(exception, "Requested Url '{RequestedUrl}' has timed out.", requestedUri);

        var extensions = new Dictionary<string, object>
        {
            ["Trace"] = httpContext.TraceIdentifier,
            ["Endpoint"] = requestedUri
        };

        var problems = Results.Problem(
            statusCode: StatusCodes.Status408RequestTimeout,
            detail: exception.Message,
            title: "The request took too long to complete and was cancelled.",
            extensions: extensions);

        httpContext.Response.StatusCode = StatusCodes.Status408RequestTimeout;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(_jsonHandler.Serialize(problems), cancellationToken);

        return true;
    }
}