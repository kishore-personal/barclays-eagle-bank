namespace EagleBank.Application.Accounts.ListAccounts;

public interface IListAccountsHandler
{
    Task<ListBankAccountsResponse> HandleAsync(ListAccountsQuery query, CancellationToken cancellationToken);
}
