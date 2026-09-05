using EagleBank.Api.Authentication;
using EagleBank.Application.Users.CreateUser;
using EagleBank.Application.Users.GetUser;
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
        var response = await _getUser.HandleAsync(
            new GetUserQuery(userId, CallerUser.RequireId(User)),
            cancellationToken);
        return Ok(response);
    }
}
