namespace EagleBank.Domain.Exceptions;

public abstract class EagleBankException : Exception
{
    public string ReasonCode { get; }

    public string SafeMessage { get; }

    protected EagleBankException(string safeMessage, string reasonCode)
        : base(safeMessage)
    {
        SafeMessage = safeMessage;
        ReasonCode = reasonCode;
    }
}
