using System.Security.Cryptography;

namespace EagleBank.Infrastructure.Identity;

public sealed class AccountNumberFactory
{
    public string Next()
    {
        return $"01{RandomNumberGenerator.GetInt32(0, 1_000_000):D6}";
    }
}
