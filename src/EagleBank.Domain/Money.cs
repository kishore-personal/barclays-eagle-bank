using EagleBank.Domain.Exceptions;

namespace EagleBank.Domain;

public readonly record struct Money
{
    public const string GbpCurrency = "GBP";
    public const int Scale = 2;
    public const long MaxPence = 1_000_000;

    public decimal Amount { get; }

    public string Currency { get; }

    public long Pence { get; }

    public static Money Zero { get; } = FromPence(0);

    private Money(long pence, string currency)
    {
        Pence = pence;
        Currency = currency;
        Amount = pence / 100m;
    }

    public static Money FromPence(long pence, string currency = GbpCurrency)
    {
        EnsureGbp(currency);
        EnsurePenceRange(pence);
        return new Money(pence, GbpCurrency);
    }

    public static Money FromMajorUnits(decimal amount, string currency = GbpCurrency)
    {
        EnsureGbp(currency);

        if (decimal.Round(amount, Scale) != amount)
        {
            throw new InvalidAmountScaleException();
        }

        var pence = decimal.ToInt64(amount * 100);
        EnsurePenceRange(pence);
        return new Money(pence, GbpCurrency);
    }

    private static void EnsureGbp(string currency)
    {
        if (!string.Equals(currency, GbpCurrency, StringComparison.Ordinal))
        {
            throw new InvalidAmountException("InvalidCurrency", "Currency must be GBP.");
        }
    }

    private static void EnsurePenceRange(long pence)
    {
        if (pence < 0 || pence > MaxPence)
        {
            throw new InvalidAmountException("AmountOutOfRange", "Amount must be between 0.00 and 10000.00.");
        }
    }
}
