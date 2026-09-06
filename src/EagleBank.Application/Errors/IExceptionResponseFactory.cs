namespace EagleBank.Application.Errors;

public interface IExceptionResponseFactory
{
    MappedErrorResponse Create(Exception exception);
}
