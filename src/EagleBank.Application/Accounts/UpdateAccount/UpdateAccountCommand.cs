namespace EagleBank.Application.Accounts.UpdateAccount;

public sealed record UpdateAccountCommand(string AccountNumber, string CallerUserId, UpdateAccountRequest Request);
