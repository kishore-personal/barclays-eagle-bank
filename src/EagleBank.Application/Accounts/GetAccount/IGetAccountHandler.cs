namespace EagleBank.Application.Accounts.GetAccount;

public interface IGetAccountHandler
{
    Task<BankAccountResponse> HandleAsync(GetAccountQuery query, CancellationToken cancellationToken);
}
