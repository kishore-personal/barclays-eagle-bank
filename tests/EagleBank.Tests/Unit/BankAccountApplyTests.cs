using EagleBank.Domain;
using EagleBank.Domain.Exceptions;

namespace EagleBank.Tests.Unit;

public class BankAccountApplyTests
{
    [Fact]
    public void Deposit_increases_balance()
    {
        var account = NewAccount();
        var now = DateTimeOffset.UtcNow;

        account.Apply(new Transaction("tan-abc123", account.AccountNumber, account.UserId, Money.FromMajorUnits(10.50m), TransactionType.Deposit, null, now));

        Assert.Equal(1050, account.Balance.Pence);
        Assert.Equal(now, account.UpdatedTimestamp);
        Assert.Single(account.Transactions);
    }

    [Fact]
    public void Deposit_over_cap_is_rejected()
    {
        var account = NewAccount();
        account.Apply(new Transaction("tan-full1", account.AccountNumber, account.UserId, Money.FromMajorUnits(10000.00m), TransactionType.Deposit, null, DateTimeOffset.UtcNow));

        Assert.Throws<BalanceCapException>(() =>
            account.Apply(new Transaction("tan-over1", account.AccountNumber, account.UserId, Money.FromMajorUnits(0.01m), TransactionType.Deposit, null, DateTimeOffset.UtcNow)));
        Assert.Equal(1_000_000, account.Balance.Pence);
        Assert.Single(account.Transactions);
    }

    [Fact]
    public void Withdrawal_decreases_balance()
    {
        var account = NewAccount();
        var now = DateTimeOffset.UtcNow;
        account.Apply(new Transaction("tan-dep1", account.AccountNumber, account.UserId, Money.FromMajorUnits(10.50m), TransactionType.Deposit, null, now));

        account.Apply(new Transaction("tan-wd1", account.AccountNumber, account.UserId, Money.FromMajorUnits(3.00m), TransactionType.Withdrawal, null, now));

        Assert.Equal(750, account.Balance.Pence);
        Assert.Equal(2, account.Transactions.Count);
    }

    [Fact]
    public void Withdrawal_without_funds_is_rejected()
    {
        var account = NewAccount();

        Assert.Throws<InsufficientFundsException>(() =>
            account.Apply(new Transaction("tan-empty1", account.AccountNumber, account.UserId, Money.FromMajorUnits(1.00m), TransactionType.Withdrawal, null, DateTimeOffset.UtcNow)));
        Assert.Equal(0, account.Balance.Pence);
        Assert.Empty(account.Transactions);
    }

    private static BankAccount NewAccount()
    {
        return new BankAccount("01234567", "usr-owner1", "Personal Bank Account", DateTimeOffset.UtcNow);
    }
}
