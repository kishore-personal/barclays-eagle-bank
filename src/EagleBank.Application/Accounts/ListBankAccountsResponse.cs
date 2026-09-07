namespace EagleBank.Application.Accounts;

public sealed record ListBankAccountsResponse(IReadOnlyList<BankAccountResponse> Accounts);
