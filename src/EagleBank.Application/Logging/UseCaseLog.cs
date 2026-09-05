using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Logging;

public static class UseCaseLog
{
    public static void Started(ILogger logger, string useCase)
    {
        logger.LogInformation("Use case started {UseCase}", useCase);
    }

    public static void Completed(ILogger logger, string useCase, string resultCode)
    {
        logger.LogInformation("Use case completed {UseCase} {ResultCode}", useCase, resultCode);
    }
}
