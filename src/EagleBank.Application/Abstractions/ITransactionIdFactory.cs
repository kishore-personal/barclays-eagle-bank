namespace EagleBank.Application.Abstractions;

public interface ITransactionIdFactory
{
    string Next();
}
