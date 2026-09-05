using EagleBank.Api.Authentication;
using EagleBank.Application.Transactions.CreateTransaction;
using EagleBank.Application.Transactions.GetTransaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EagleBank.Api.Controllers;

[ApiController]
[Authorize]
[Route("v1/accounts/{accountNumber}/transactions")]
public sealed class TransactionsController : ControllerBase
{
    private readonly ICreateTransactionHandler _createTransaction;
    private readonly IGetTransactionHandler _getTransaction;

    public TransactionsController(
        ICreateTransactionHandler createTransaction,
        IGetTransactionHandler getTransaction)
    {
        _createTransaction = createTransaction;
        _getTransaction = getTransaction;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(
        string accountNumber,
        [FromBody] CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createTransaction.HandleAsync(
            new CreateTransactionCommand(accountNumber, CallerUser.RequireId(User), request),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransaction(
        string accountNumber,
        string transactionId,
        CancellationToken cancellationToken)
    {
        var response = await _getTransaction.HandleAsync(
            new GetTransactionQuery(accountNumber, transactionId, CallerUser.RequireId(User)),
            cancellationToken);
        return Ok(response);
    }
}
