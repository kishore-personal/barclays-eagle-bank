using EagleBank.Api.Authentication;
using EagleBank.Application.Accounts.CreateAccount;
using EagleBank.Application.Accounts.GetAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EagleBank.Api.Controllers;

[ApiController]
[Authorize]
[Route("v1/accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly ICreateAccountHandler _createAccount;
    private readonly IGetAccountHandler _getAccount;

    public AccountsController(ICreateAccountHandler createAccount, IGetAccountHandler getAccount)
    {
        _createAccount = createAccount;
        _getAccount = getAccount;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createAccount.HandleAsync(
            new CreateAccountCommand(CallerUser.RequireId(User), request),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{accountNumber}")]
    public async Task<IActionResult> GetAccount(string accountNumber, CancellationToken cancellationToken)
    {
        var response = await _getAccount.HandleAsync(
            new GetAccountQuery(accountNumber, CallerUser.RequireId(User)),
            cancellationToken);
        return Ok(response);
    }
}
