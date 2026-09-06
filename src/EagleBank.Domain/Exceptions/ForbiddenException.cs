namespace EagleBank.Domain.Exceptions;

public sealed class ForbiddenException : EagleBankException
{
    public ForbiddenException()
        : base("The caller is not allowed to access this resource.", "Forbidden")
    {
    }
}
