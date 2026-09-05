using Microsoft.Extensions.Logging;

namespace EagleBank.Domain.Logging;

public static class InvariantLog
{
    public static void Rejected(ILogger logger, string reasonCode)
    {
        logger.LogInformation("Domain invariant rejected {ReasonCode}", reasonCode);
    }
}
