namespace EagleBank.Api.Middleware;

public sealed class RequestContextMiddleware
{
    public const string RequestIdItemKey = "RequestId";
    public const string RequestIdHeaderName = "X-Request-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestContextMiddleware> _logger;

    public RequestContextMiddleware(RequestDelegate next, ILogger<RequestContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers[RequestIdHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(requestId))
        {
            requestId = Guid.NewGuid().ToString("N");
        }

        context.Items[RequestIdItemKey] = requestId;

        using (_logger.BeginScope(new Dictionary<string, object?> { ["RequestId"] = requestId }))
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[RequestIdHeaderName] = requestId;
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
