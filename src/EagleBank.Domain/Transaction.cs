namespace EagleBank.Domain;

public sealed class Transaction
{
    public string Id { get; private set; } = null!;

    public string AccountNumber { get; private set; } = null!;

    public BankAccount Account { get; private set; } = null!;

    public string UserId { get; private set; } = null!;

    public Money Amount { get; private set; }

    public string Currency { get; private set; } = null!;

    public TransactionType Type { get; private set; }

    public string? Reference { get; private set; }

    public DateTimeOffset CreatedTimestamp { get; private set; }

    private Transaction()
    {
    }

    public Transaction(
        string id,
        string accountNumber,
        string userId,
        Money amount,
        TransactionType type,
        string? reference,
        DateTimeOffset createdTimestamp)
    {
        Id = id;
        AccountNumber = accountNumber;
        UserId = userId;
        Amount = amount;
        Currency = Money.GbpCurrency;
        Type = type;
        Reference = reference;
        CreatedTimestamp = createdTimestamp;
    }
}
