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

    public async Task<IReadOnlyList<BankAccount>> ListByUserIdAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var accounts = await _db.Accounts
            .AsNoTracking()
            .Where(account => account.UserId == userId)
            .ToListAsync(cancellationToken);
        return accounts
            .OrderBy(account => account.CreatedTimestamp)
            .ToArray();
    }

    public Task<bool> HasAnyForUserAsync(string userId, CancellationToken cancellationToken)
    {
        return _db.Accounts.AnyAsync(account => account.UserId == userId, cancellationToken);
    }

    public async Task UpdateAsync(BankAccount account, CancellationToken cancellationToken)
    {
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string accountNumber, CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        await _db.Transactions
            .Where(item => item.AccountNumber == accountNumber)
            .ExecuteDeleteAsync(cancellationToken);
        await _db.Accounts
            .Where(account => account.AccountNumber == accountNumber)
            .ExecuteDeleteAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static bool IsUniqueConstraint(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException sqlite && sqlite.SqliteErrorCode == 19;
    }
}
