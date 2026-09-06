using System.Security.Claims;

namespace EagleBank.Api.Middleware;

public sealed class AuthenticatedUserScopeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticatedUserScopeMiddleware> _logger;

    public AuthenticatedUserScopeMiddleware(
        RequestDelegate next,
        ILogger<AuthenticatedUserScopeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirstValue("sub") ?? context.User.FindFirstValue("userId");
        if (string.IsNullOrWhiteSpace(userId) || !userId.StartsWith("usr-", StringComparison.Ordinal))
        {
            await _next(context);
            return;
        }

        using (_logger.BeginScope(new Dictionary<string, object?> { ["userId"] = userId }))
        {
            await _next(context);
        }
    }
}
