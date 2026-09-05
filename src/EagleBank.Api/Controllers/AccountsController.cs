using EagleBank.Api.Authentication;
using EagleBank.Application.Accounts.CreateAccount;
using EagleBank.Application.Accounts.DeleteAccount;
using EagleBank.Application.Accounts.GetAccount;
using EagleBank.Application.Accounts.ListAccounts;
using EagleBank.Application.Accounts.UpdateAccount;
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
    private readonly IListAccountsHandler _listAccounts;
    private readonly IUpdateAccountHandler _updateAccount;
    private readonly IDeleteAccountHandler _deleteAccount;

    public AccountsController(
        ICreateAccountHandler createAccount,
        IGetAccountHandler getAccount,
        IListAccountsHandler listAccounts,
        IUpdateAccountHandler updateAccount,
        IDeleteAccountHandler deleteAccount)
    {
        _createAccount = createAccount;
        _getAccount = getAccount;
        _listAccounts = listAccounts;
        _updateAccount = updateAccount;
        _deleteAccount = deleteAccount;
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

    [HttpGet]
    public async Task<IActionResult> ListAccounts(CancellationToken cancellationToken)
    {
        var response = await _listAccounts.HandleAsync(
            new ListAccountsQuery(CallerUser.RequireId(User)),
            cancellationToken);
        return Ok(response);
    }

    [HttpGet("{accountNumber}")]
    public async Task<IActionResult> GetAccount(string accountNumber, CancellationToken cancellationToken)
    {
        var response = await _getAccount.HandleAsync(
            new GetAccountQuery(accountNumber, CallerUser.RequireId(User)),
            cancellationToken);
        return Ok(response);
    }

    [HttpPatch("{accountNumber}")]
    public async Task<IActionResult> UpdateAccount(
        string accountNumber,
        [FromBody] UpdateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _updateAccount.HandleAsync(
            new UpdateAccountCommand(accountNumber, CallerUser.RequireId(User), request),
            cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{accountNumber}")]
    public async Task<IActionResult> DeleteAccount(string accountNumber, CancellationToken cancellationToken)
    {
        await _deleteAccount.HandleAsync(
            new DeleteAccountCommand(accountNumber, CallerUser.RequireId(User)),
            cancellationToken);
        return NoContent();
    }
}
