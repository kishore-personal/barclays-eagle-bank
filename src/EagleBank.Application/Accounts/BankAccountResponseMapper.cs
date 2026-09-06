using EagleBank.Domain;

namespace EagleBank.Application.Accounts;

public static class BankAccountResponseMapper
{
    public static BankAccountResponse ToResponse(BankAccount account)
    {
        return new BankAccountResponse(
            account.AccountNumber,
            account.SortCode,
            account.Name,
            account.AccountType,
            account.Balance.Amount,
            account.Currency,
            account.CreatedTimestamp,
            account.UpdatedTimestamp);
    }
}
