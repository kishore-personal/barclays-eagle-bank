using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EagleBank.Application.Users.CreateUser;
using EagleBank.Application.Users.GetUser;
using EagleBank.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EagleBank.Api.Controllers;

[ApiController]
[Authorize]
[Route("v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ICreateUserHandler _createUser;
    private readonly IGetUserHandler _getUser;

    public UsersController(ICreateUserHandler createUser, IGetUserHandler getUser)
    {
        _createUser = createUser;
        _getUser = getUser;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createUser.HandleAsync(new CreateUserCommand(request), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId, CancellationToken cancellationToken)
    {
        var callerUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(callerUserId))
        {
            throw new UnauthorizedException();
        }

        var response = await _getUser.HandleAsync(new GetUserQuery(userId, callerUserId), cancellationToken);
        return Ok(response);
    }
}
