namespace EagleBank.Domain.Exceptions;

public sealed class UnauthorizedException : EagleBankException
{
    public UnauthorizedException()
        : base("Invalid credentials.", "Unauthorized")
    {
    }
}
