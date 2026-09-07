namespace EagleBank.Application.Errors;

public sealed record ErrorDetail(string Field, string Message, string Type);
