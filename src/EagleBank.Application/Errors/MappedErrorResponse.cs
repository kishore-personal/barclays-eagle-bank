namespace EagleBank.Application.Errors;

public sealed record MappedErrorResponse(int StatusCode, object Body);
