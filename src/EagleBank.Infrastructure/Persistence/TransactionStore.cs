using EagleBank.Application.Abstractions;
using EagleBank.Domain;
using EagleBank.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EagleBank.Infrastructure.Persistence;

public sealed class TransactionStore : ITransactionStore
{
    private readonly EagleBankDbContext _db;

    public TransactionStore(EagleBankDbContext db)
    {
        _db = db;
    }

    public async Task AddAtomicAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        await using var dbTransaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        var rows = transaction.Type == TransactionType.Deposit
            ? await TryDepositAsync(transaction, cancellationToken)
            : await TryWithdrawAsync(transaction, cancellationToken);

        if (rows == 0)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw transaction.Type == TransactionType.Deposit
                ? new BalanceCapException()
                : new InsufficientFundsException();
        }

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync(cancellationToken);
        await dbTransaction.CommitAsync(cancellationToken);
    }

    public Task<Transaction?> GetByIdAsync(string transactionId, CancellationToken cancellationToken)
    {
        return _db.Transactions
            .AsNoTracking()
            .SingleOrDefaultAsync(transaction => transaction.Id == transactionId, cancellationToken);
    }

    private Task<int> TryDepositAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        return _db.Database.ExecuteSqlInterpolatedAsync(
            $"""
             UPDATE accounts
             SET balance_pence = balance_pence + {transaction.Amount.Pence},
                 updated_timestamp = {transaction.CreatedTimestamp}
             WHERE account_number = {transaction.AccountNumber}
               AND balance_pence + {transaction.Amount.Pence} <= {Money.MaxPence}
             """,
            cancellationToken);
    }

    private Task<int> TryWithdrawAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        return _db.Database.ExecuteSqlInterpolatedAsync(
            $"""
             UPDATE accounts
             SET balance_pence = balance_pence - {transaction.Amount.Pence},
                 updated_timestamp = {transaction.CreatedTimestamp}
             WHERE account_number = {transaction.AccountNumber}
               AND balance_pence >= {transaction.Amount.Pence}
             """,
            cancellationToken);
    }
}
