using Microsoft.Extensions.Logging;

namespace EagleBank.Infrastructure.Logging;

public static class JwtIssueLog
{
    public static void Issued(ILogger logger)
    {
        logger.LogInformation("JWT issued");
    }
}
