namespace EagleBank.Domain.Exceptions;

public sealed class InsufficientFundsException : EagleBankException
{
    public InsufficientFundsException()
        : base("Insufficient funds to process the transaction.", "InsufficientFunds")
    {
    }
}
