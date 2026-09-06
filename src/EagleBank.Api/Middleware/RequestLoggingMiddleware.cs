using System.Diagnostics;

namespace EagleBank.Api.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var started = Stopwatch.GetTimestamp();
        try
        {
            await _next(context);
        }
        finally
        {
            var elapsedMs = (int)Stopwatch.GetElapsedTime(started).TotalMilliseconds;
            var routeTemplate = context.GetEndpoint() is RouteEndpoint route
                ? route.RoutePattern.RawText
                : "unmatched";

            _logger.LogInformation(
                "HTTP request completed {Method} {RouteTemplate} {StatusCode} {ElapsedMs}",
                context.Request.Method,
                routeTemplate,
                context.Response.StatusCode,
                elapsedMs);
        }
    }
}
