using System.Text.Json.Serialization;

namespace EagleBank.Application.Transactions;

public sealed record TransactionResponse(
    string Id,
    decimal Amount,
    string Currency,
    string Type,
    DateTimeOffset CreatedTimestamp,
    string UserId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Reference);
