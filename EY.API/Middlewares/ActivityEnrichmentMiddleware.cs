using System.Diagnostics;
using EY.Shared.Contracts;
using Microsoft.AspNetCore.Http.Features;

namespace EY.API.Middlewares;

public class ActivityEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public ActivityEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUrlHelper urlHelper)
    {
        var act = context.Features.Get<IHttpActivityFeature>();
        // Verifica se há uma atividade atual
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetTag("http.method", context.Request.Method);
            activity.SetTag("http.endpoint", urlHelper.GetDisplayUrl(context.Request));
            activity.SetTag("client.ip", context.Connection.RemoteIpAddress?.ToString());
            activity.SetTag("user.agent", context.Request.Headers["User-Agent"].ToString());

            // Inclui o ID de correlação, se estiver presente nos cabeçalhos
            if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
                activity.SetTag("correlation_id", correlationId.ToString());

            // Informações do usuário, se autenticado
            if (context.User is { Identity: { IsAuthenticated: true } })
            {
                activity.SetTag("user.id", context.User.FindFirst("sub")?.Value);
                activity.SetTag("user.name", context.User.Identity.Name);
                activity.SetTag("user.role", string.Join(",", context.User.Claims
                    .Where(c => c.Type == "role")
                    .Select(c => c.Value)));
            }
        }

        // Chama o próximo middleware
        await _next(context);
    }
}