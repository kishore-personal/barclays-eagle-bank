using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EagleBank.Domain.Exceptions;

namespace EagleBank.Api.Authentication;

public static class CallerUser
{
    public static string RequireId(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException();
        }

        return userId;
    }
}
