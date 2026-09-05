using EagleBank.Api.Authentication;
using EagleBank.Application.Users.CreateUser;
using EagleBank.Application.Users.DeleteUser;
using EagleBank.Application.Users.GetUser;
using EagleBank.Application.Users.UpdateUser;
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
    private readonly IUpdateUserHandler _updateUser;
    private readonly IDeleteUserHandler _deleteUser;

    public UsersController(
        ICreateUserHandler createUser,
        IGetUserHandler getUser,
        IUpdateUserHandler updateUser,
        IDeleteUserHandler deleteUser)
    {
        _createUser = createUser;
        _getUser = getUser;
        _updateUser = updateUser;
        _deleteUser = deleteUser;
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

    [HttpPatch("{userId}")]
    public async Task<IActionResult> UpdateUser(
        string userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _updateUser.HandleAsync(
            new UpdateUserCommand(userId, CallerUser.RequireId(User), request),
            cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId, CancellationToken cancellationToken)
    {
        await _deleteUser.HandleAsync(
            new DeleteUserCommand(userId, CallerUser.RequireId(User)),
            cancellationToken);
        return NoContent();
    }
}
