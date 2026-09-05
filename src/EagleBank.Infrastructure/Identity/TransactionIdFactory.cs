using System.Security.Cryptography;
using EagleBank.Application.Abstractions;

namespace EagleBank.Infrastructure.Identity;

public sealed class TransactionIdFactory : ITransactionIdFactory
{
    public string Next()
    {
        return "tan-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
    }
}
