using System.Security.Cryptography;
using EagleBank.Application.Abstractions;

namespace EagleBank.Infrastructure.Identity;

public sealed class UserIdFactory : IUserIdFactory
{
    public string Next()
    {
        return "usr-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
    }
}
