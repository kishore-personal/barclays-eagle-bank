using Microsoft.Extensions.Logging;

namespace EagleBank.Infrastructure.Logging;

public static class PersistenceLog
{
    public static void Committed(ILogger logger, string entityType, int rowsAffected)
    {
        logger.LogInformation("Persist committed {EntityType} {RowsAffected}", entityType, rowsAffected);
    }

    public static void RolledBack(ILogger logger, string entityType)
    {
        logger.LogInformation("Persist rolled back {EntityType}", entityType);
    }
}
