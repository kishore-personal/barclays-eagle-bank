namespace EagleBank.Domain.Exceptions;

public sealed class InvalidAmountScaleException : EagleBankException
{
    public InvalidAmountScaleException()
        : base("Amount must have at most two decimal places.", "InvalidAmountScale")
    {
    }
}
