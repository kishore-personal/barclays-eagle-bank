using EagleBank.Application.Abstractions;
using EagleBank.Domain;
using EagleBank.Infrastructure.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EagleBank.Infrastructure.Persistence;

public sealed class AccountStore : IAccountStore
{
    private const int MaxAllocationAttempts = 8;

    private readonly EagleBankDbContext _db;
    private readonly AccountNumberFactory _accountNumbers;

    public AccountStore(EagleBankDbContext db, AccountNumberFactory accountNumbers)
    {
        _db = db;
        _accountNumbers = accountNumbers;
    }

    public async Task<BankAccount> AddAsync(string userId, string name, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxAllocationAttempts; attempt++)
        {
            var account = new BankAccount(_accountNumbers.Next(), userId, name, DateTimeOffset.UtcNow);
            _db.Accounts.Add(account);
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return account;
            }
            catch (DbUpdateException exception) when (IsUniqueConstraint(exception))
            {
                _db.Entry(account).State = EntityState.Detached;
            }
        }

        throw new InvalidOperationException("Unable to allocate a unique account number.");
    }

    public Task<BankAccount?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken)
    {
        return _db.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(account => account.AccountNumber == accountNumber, cancellationToken);
    }

    private static bool IsUniqueConstraint(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException sqlite && sqlite.SqliteErrorCode == 19;
    }
}
