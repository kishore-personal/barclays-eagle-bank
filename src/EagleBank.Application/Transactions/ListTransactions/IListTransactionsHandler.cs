namespace EagleBank.Application.Transactions.ListTransactions;

public interface IListTransactionsHandler
{
    Task<ListTransactionsResponse> HandleAsync(ListTransactionsQuery query, CancellationToken cancellationToken);
}
