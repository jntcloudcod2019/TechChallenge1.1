using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TechChallengeFIAP.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Log request details
            Log.Information("Handling request: {Method} {Url}",
                context.Request.Method, context.Request.Path);

            await _next(context);

            stopwatch.Stop();

            // Log response details
            Log.Information("Finished handling request in {ElapsedMilliseconds} ms",
                stopwatch.ElapsedMilliseconds);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
