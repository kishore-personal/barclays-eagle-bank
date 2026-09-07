namespace EagleBank.Domain.Exceptions;

public sealed class ConflictException : EagleBankException
{
    public ConflictException()
        : base("The request conflicts with the current state of the resource.", "Conflict")
    {
    }
}
