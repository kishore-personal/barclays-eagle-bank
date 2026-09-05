using EagleBank.Application.Auth.Login;
using Microsoft.AspNetCore.Mvc;

namespace EagleBank.Api.Controllers;

[ApiController]
[Route("v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ILoginHandler _login;

    public AuthController(ILoginHandler login)
    {
        _login = login;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _login.HandleAsync(new LoginCommand(request), cancellationToken);
        return Ok(response);
    }
}
