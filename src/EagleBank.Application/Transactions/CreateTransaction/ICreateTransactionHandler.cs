namespace EagleBank.Application.Transactions.CreateTransaction;

public interface ICreateTransactionHandler
{
    Task<TransactionResponse> HandleAsync(CreateTransactionCommand command, CancellationToken cancellationToken);
}
