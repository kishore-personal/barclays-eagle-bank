using System.Text.Json;
using EagleBank.Application.Errors;
using Microsoft.AspNetCore.Diagnostics;

namespace EagleBank.Api.ExceptionHandling;

public sealed class EagleBankExceptionHandler : IExceptionHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IExceptionResponseFactory _factory;
    private readonly ILogger<EagleBankExceptionHandler> _logger;

    public EagleBankExceptionHandler(
        IExceptionResponseFactory factory,
        ILogger<EagleBankExceptionHandler> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var mapped = _factory.Create(exception);

        _logger.LogWarning(
            "Mapped exception {ExceptionType} to {StatusCode}",
            exception.GetType().Name,
            mapped.StatusCode);

        httpContext.Response.StatusCode = mapped.StatusCode;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(mapped.Body, JsonOptions, cancellationToken);
        return true;
    }
}
