namespace EagleBank.Application.Errors;

public sealed record BadRequestErrorResponse(string Message, IReadOnlyList<ErrorDetail> Details);
