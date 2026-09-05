using EagleBank.Domain.Exceptions;

namespace EagleBank.Application.Authorization;

public static class ResourceOwnership
{
    public static T RequireOwned<T>(T? resource, string callerUserId, Func<T, string> ownerId)
    {
        if (resource is null)
        {
            throw new NotFoundException();
        }

        if (!string.Equals(ownerId(resource), callerUserId, StringComparison.Ordinal))
        {
            throw new ForbiddenException();
        }

        return resource;
    }
}
