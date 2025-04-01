using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IUrlHelper = EY.Shared.Contracts.IUrlHelper;

namespace EY.API.Middlewares;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> _logger,
    IUrlHelper _urlHelper,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is null)
            return false;

        var requestedUri = _urlHelper.GetDisplayUrl(httpContext.Request);
        _logger.LogError(exception, "Exception occurred at {RequestedUrl}: {Message} and was caught by global handler.",
            requestedUri, exception.Message);

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Detail = exception.Message
            }
        });
    }
}