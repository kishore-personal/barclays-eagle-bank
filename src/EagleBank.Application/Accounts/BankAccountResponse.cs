namespace EagleBank.Application.Accounts;

public sealed record BankAccountResponse(
    string AccountNumber,
    string SortCode,
    string Name,
    string AccountType,
    decimal Balance,
    string Currency,
    DateTimeOffset CreatedTimestamp,
    DateTimeOffset UpdatedTimestamp);
