using EagleBank.Domain;

namespace EagleBank.Application.Abstractions;

public interface ITransactionStore
{
    Task AddAtomicAsync(Transaction transaction, CancellationToken cancellationToken);

    Task<Transaction?> GetByIdAsync(string transactionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Transaction>> ListByAccountNumberAsync(
        string accountNumber,
        CancellationToken cancellationToken);
}
