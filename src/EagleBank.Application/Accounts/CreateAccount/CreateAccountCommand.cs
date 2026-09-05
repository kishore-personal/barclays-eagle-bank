namespace EagleBank.Application.Accounts.CreateAccount;

public sealed record CreateAccountCommand(string CallerUserId, CreateAccountRequest Request);
