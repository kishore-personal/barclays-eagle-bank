using EagleBank.Application.Authorization;
using EagleBank.Domain.Exceptions;

namespace EagleBank.Tests.Unit;

public class ResourceOwnershipTests
{
    [Fact]
    public void Missing_resource_is_not_found()
    {
        Assert.Throws<NotFoundException>(() =>
            ResourceOwnership.RequireOwned<string>(null, "usr-caller", value => value));
    }

    [Fact]
    public void Other_owner_is_forbidden()
    {
        Assert.Throws<ForbiddenException>(() =>
            ResourceOwnership.RequireOwned("usr-other", "usr-caller", value => value));
    }

    [Fact]
    public void Owner_receives_the_resource()
    {
        var owned = ResourceOwnership.RequireOwned("usr-caller", "usr-caller", value => value);

        Assert.Equal("usr-caller", owned);
    }
}
