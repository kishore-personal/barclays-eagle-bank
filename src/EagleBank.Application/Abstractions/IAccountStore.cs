using EagleBank.Domain;

namespace EagleBank.Application.Abstractions;

public interface IAccountStore
{
    Task<BankAccount> AddAsync(string userId, string name, CancellationToken cancellationToken);

    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken);

    Task<IReadOnlyList<BankAccount>> ListByUserIdAsync(string userId, CancellationToken cancellationToken);

    Task<bool> HasAnyForUserAsync(string userId, CancellationToken cancellationToken);

    Task UpdateAsync(BankAccount account, CancellationToken cancellationToken);

    Task DeleteAsync(string accountNumber, CancellationToken cancellationToken);
}
