namespace EagleBank.Tests.Support;

public static class TransactionFixtures
{
    public const string Reference = "Monthly rent";
    public const string DepositAmountText = "10.50";
    public const string InvalidAmountText = "10.999";
    public const string CapAmountText = "10000.00";
    public const string OverCapAmountText = "0.01";

    public static object DepositBody(decimal amount) => new
    {
        amount,
        currency = "GBP",
        type = "deposit",
        reference = Reference
    };
}
