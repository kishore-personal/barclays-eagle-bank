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
}
