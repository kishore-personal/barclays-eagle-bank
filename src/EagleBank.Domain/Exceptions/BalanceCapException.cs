namespace EagleBank.Domain.Exceptions;

public sealed class BalanceCapException : EagleBankException
{
    public BalanceCapException()
        : base("The deposit would exceed the account balance limit.", "BalanceCap")
    {
    }
}
