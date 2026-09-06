namespace EagleBank.Domain.Exceptions;

public sealed class NotFoundException : EagleBankException
{
    public NotFoundException()
        : base("Resource was not found.", "NotFound")
    {
    }
}
