using EagleBank.Domain;
using EagleBank.Domain.Exceptions;

namespace EagleBank.Tests.Unit;

public class MoneyTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(10.99, 1099)]
    [InlineData(10000.00, 1_000_000)]
    [InlineData(10.9, 1090)]
    public void FromMajorUnits_accepts_two_decimal_gbp(decimal amount, long expectedPence)
    {
        var money = Money.FromMajorUnits(amount);

        Assert.Equal(expectedPence, money.Pence);
        Assert.Equal(Money.GbpCurrency, money.Currency);
        Assert.Equal(Money.FromPence(expectedPence).Amount, money.Amount);
    }

    [Fact]
    public void FromPence_round_trips_major_units()
    {
        var money = Money.FromPence(259);

        Assert.Equal(2.59m, money.Amount);
        Assert.Equal(259, Money.FromMajorUnits(money.Amount).Pence);
    }

    [Fact]
    public void Zero_is_zero_pence()
    {
        Assert.Equal(0, Money.Zero.Pence);
        Assert.Equal(0.00m, Money.Zero.Amount);
    }

    [Fact]
    public void FromMajorUnits_rejects_more_than_two_decimals()
    {
        var exception = Assert.Throws<InvalidAmountScaleException>(() => Money.FromMajorUnits(10.999m));
        Assert.Equal("InvalidAmountScale", exception.ReasonCode);
    }

    [Fact]
    public void FromMajorUnits_rejects_non_gbp()
    {
        var exception = Assert.Throws<InvalidAmountException>(() => Money.FromMajorUnits(1.00m, "USD"));
        Assert.Equal("InvalidCurrency", exception.ReasonCode);
    }

    [Fact]
    public void FromMajorUnits_rejects_negative()
    {
        var exception = Assert.Throws<InvalidAmountException>(() => Money.FromMajorUnits(-0.01m));
        Assert.Equal("AmountOutOfRange", exception.ReasonCode);
    }

    [Fact]
    public void FromMajorUnits_rejects_above_cap()
    {
        var exception = Assert.Throws<InvalidAmountException>(() => Money.FromMajorUnits(10000.01m));
        Assert.Equal("AmountOutOfRange", exception.ReasonCode);
    }
}
