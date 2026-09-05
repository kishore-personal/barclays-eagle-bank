using EagleBank.Domain.Exceptions;
using FluentValidation;

namespace EagleBank.Application.Errors;

public sealed class ExceptionResponseFactory : IExceptionResponseFactory
{
    public const string UnexpectedErrorMessage = "An unexpected error occurred.";
    public const string ValidationFailedMessage = "The request was invalid.";

    public MappedErrorResponse Create(Exception exception)
    {
        return exception switch
        {
            ValidationException validation => MapValidation(validation),
            NotFoundException e => MapSafe(404, e),
            ForbiddenException e => MapSafe(403, e),
            ConflictException e => MapSafe(409, e),
            InsufficientFundsException e => MapSafe(422, e),
            BalanceCapException e => MapSafe(422, e),
            EagleBankException e => MapSafe(400, e),
            _ => new MappedErrorResponse(500, new ErrorResponse(UnexpectedErrorMessage))
        };
    }

    private static MappedErrorResponse MapSafe(int statusCode, EagleBankException exception)
    {
        return new MappedErrorResponse(statusCode, new ErrorResponse(exception.SafeMessage));
    }

    private static MappedErrorResponse MapValidation(ValidationException exception)
    {
        var details = exception.Errors
            .Select(error => new ErrorDetail(
                error.PropertyName,
                error.ErrorMessage,
                string.IsNullOrWhiteSpace(error.ErrorCode) ? "Validation" : error.ErrorCode))
            .ToArray();

        return new MappedErrorResponse(400, new BadRequestErrorResponse(ValidationFailedMessage, details));
    }
}
