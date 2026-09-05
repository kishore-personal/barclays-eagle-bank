using System.Text.Json;
using EagleBank.Api.ExceptionHandling;
using EagleBank.Application.Errors;
using EagleBank.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace EagleBank.Tests.Unit;

public class EagleBankExceptionHandlerTests
{
    [Fact]
    public async Task Writes_not_found_json_without_stack()
    {
        var json = await HandleAsync(new NotFoundException());

        Assert.Equal(404, json.StatusCode);
        Assert.Equal("Resource was not found.", json.Document.RootElement.GetProperty("message").GetString());
        Assert.DoesNotContain("at ", json.Raw, StringComparison.Ordinal);
        Assert.DoesNotContain("Stack", json.Raw, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Writes_safe_500_for_unknown_exception()
    {
        var json = await HandleAsync(new InvalidOperationException("password=super-secret"));

        Assert.Equal(500, json.StatusCode);
        Assert.Equal(ExceptionResponseFactory.UnexpectedErrorMessage, json.Document.RootElement.GetProperty("message").GetString());
        Assert.DoesNotContain("password", json.Raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("secret", json.Raw, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<(int StatusCode, string Raw, JsonDocument Document)> HandleAsync(Exception exception)
    {
        var handler = new EagleBankExceptionHandler(
            new ExceptionResponseFactory(),
            NullLogger<EagleBankExceptionHandler>.Instance);

        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        context.Response.Body.Position = 0;
        var raw = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return (context.Response.StatusCode, raw, JsonDocument.Parse(raw));
    }
}
