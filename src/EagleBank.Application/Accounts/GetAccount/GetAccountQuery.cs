namespace EagleBank.Application.Accounts.GetAccount;

public sealed record GetAccountQuery(string AccountNumber, string CallerUserId);
