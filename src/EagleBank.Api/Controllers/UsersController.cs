using EagleBank.Application.Users.CreateUser;
using Microsoft.AspNetCore.Mvc;

namespace EagleBank.Api.Controllers;

[ApiController]
[Route("v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ICreateUserHandler _createUser;

    public UsersController(ICreateUserHandler createUser)
    {
        _createUser = createUser;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createUser.HandleAsync(new CreateUserCommand(request), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }
}
