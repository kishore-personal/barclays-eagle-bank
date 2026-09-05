using EagleBank.Application.Errors;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace EagleBank.Tests.Unit;

public class ExceptionResponseFactoryTests
{
    private readonly ExceptionResponseFactory _factory = new();

    [Fact]
    public void NotFound_maps_to_404_error_response()
    {
        var mapped = _factory.Create(new NotFoundException());

        Assert.Equal(404, mapped.StatusCode);
        var body = Assert.IsType<ErrorResponse>(mapped.Body);
        Assert.Equal("Resource was not found.", body.Message);
    }

    [Fact]
    public void Forbidden_maps_to_403_error_response()
    {
        var mapped = _factory.Create(new ForbiddenException());

        Assert.Equal(403, mapped.StatusCode);
        Assert.IsType<ErrorResponse>(mapped.Body);
    }

    [Fact]
    public void Conflict_maps_to_409_error_response()
    {
        var mapped = _factory.Create(new ConflictException());

        Assert.Equal(409, mapped.StatusCode);
        Assert.IsType<ErrorResponse>(mapped.Body);
    }

    [Fact]
    public void InsufficientFunds_maps_to_422_error_response()
    {
        var mapped = _factory.Create(new InsufficientFundsException());

        Assert.Equal(422, mapped.StatusCode);
        Assert.IsType<ErrorResponse>(mapped.Body);
    }

    [Fact]
    public void BalanceCap_maps_to_422_error_response()
    {
        var mapped = _factory.Create(new BalanceCapException());

        Assert.Equal(422, mapped.StatusCode);
        Assert.IsType<ErrorResponse>(mapped.Body);
    }

    [Fact]
    public void Validation_maps_to_400_with_details()
    {
        var exception = new ValidationException([
            new ValidationFailure("email", "Email is required") { ErrorCode = "NotEmpty" }
        ]);

        var mapped = _factory.Create(exception);

        Assert.Equal(400, mapped.StatusCode);
        var body = Assert.IsType<BadRequestErrorResponse>(mapped.Body);
        Assert.Equal(ExceptionResponseFactory.ValidationFailedMessage, body.Message);
        Assert.Equal("email", body.Details[0].Field);
        Assert.Equal("Email is required", body.Details[0].Message);
        Assert.Equal("NotEmpty", body.Details[0].Type);
    }

    [Fact]
    public void Unknown_maps_to_500_without_exception_message()
    {
        var mapped = _factory.Create(new InvalidOperationException("password=super-secret stacktrace"));

        Assert.Equal(500, mapped.StatusCode);
        var body = Assert.IsType<ErrorResponse>(mapped.Body);
        Assert.Equal(ExceptionResponseFactory.UnexpectedErrorMessage, body.Message);
        Assert.DoesNotContain("password", body.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("secret", body.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stacktrace", body.Message, StringComparison.OrdinalIgnoreCase);
    }
}
