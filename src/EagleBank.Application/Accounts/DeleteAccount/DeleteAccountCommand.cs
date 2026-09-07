namespace EagleBank.Application.Accounts.DeleteAccount;

public sealed record DeleteAccountCommand(string AccountNumber, string CallerUserId);
