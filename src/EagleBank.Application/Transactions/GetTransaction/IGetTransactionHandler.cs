namespace EagleBank.Application.Transactions.GetTransaction;

public interface IGetTransactionHandler
{
    Task<TransactionResponse> HandleAsync(GetTransactionQuery query, CancellationToken cancellationToken);
}
