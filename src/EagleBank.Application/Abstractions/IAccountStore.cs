using EagleBank.Domain;

namespace EagleBank.Application.Abstractions;

public interface IAccountStore
{
    Task<BankAccount> AddAsync(string userId, string name, CancellationToken cancellationToken);

    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken);
}
