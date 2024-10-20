using Microsoft.AspNetCore.Http;
using System.Threading;

namespace EY.API.Middlewares
{
    public class SimpleAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public SimpleAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string authorization = context.Request.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(authorization) && authorization.Equals("secret"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized access! GET OUT!");

                return;
            }

            await _next(context);
        }
    }
}
