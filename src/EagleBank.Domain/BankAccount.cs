using EagleBank.Domain.Exceptions;

namespace EagleBank.Domain;

public sealed class BankAccount
{
    public const string FixedSortCode = "10-10-10";
    public const string PersonalAccountType = "personal";

    public string AccountNumber { get; private set; } = null!;

    public string UserId { get; private set; } = null!;

    public User User { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string AccountType { get; private set; } = null!;

    public string SortCode { get; private set; } = null!;

    public string Currency { get; private set; } = null!;

    public Money Balance { get; private set; }

    public DateTimeOffset CreatedTimestamp { get; private set; }

    public DateTimeOffset UpdatedTimestamp { get; private set; }

    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    private BankAccount()
    {
    }

    public BankAccount(
        string accountNumber,
        string userId,
        string name,
        DateTimeOffset createdTimestamp)
    {
        AccountNumber = accountNumber;
        UserId = userId;
        Name = name;
        AccountType = PersonalAccountType;
        SortCode = FixedSortCode;
        Currency = Money.GbpCurrency;
        Balance = Money.Zero;
        CreatedTimestamp = createdTimestamp;
        UpdatedTimestamp = createdTimestamp;
    }

    public void Apply(Transaction transaction)
    {
        if (!string.Equals(transaction.AccountNumber, AccountNumber, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Transaction does not belong to this account.");
        }

        if (transaction.Type == TransactionType.Deposit)
        {
            var next = Balance.Pence + transaction.Amount.Pence;
            if (next > Money.MaxPence)
            {
                throw new BalanceCapException();
            }

            Balance = Money.FromPence(next);
        }
        else
        {
            if (transaction.Amount.Pence > Balance.Pence)
            {
                throw new InsufficientFundsException();
            }

            Balance = Money.FromPence(Balance.Pence - transaction.Amount.Pence);
        }

        UpdatedTimestamp = transaction.CreatedTimestamp;
        Transactions.Add(transaction);
    }

    public void ApplyPartialUpdate(string? name, string? accountType, DateTimeOffset updatedTimestamp)
    {
        if (name is not null)
        {
            Name = name;
        }

        if (accountType is not null)
        {
            AccountType = accountType;
        }

        UpdatedTimestamp = updatedTimestamp;
    }
}
