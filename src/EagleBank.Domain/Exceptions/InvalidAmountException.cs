namespace EagleBank.Domain.Exceptions;

public sealed class InvalidAmountException : EagleBankException
{
    public InvalidAmountException(string reasonCode, string safeMessage)
        : base(safeMessage, reasonCode)
    {
    }
}
