using EagleBank.Api.Authentication;
using EagleBank.Application.Transactions.CreateTransaction;
using EagleBank.Application.Transactions.GetTransaction;
using EagleBank.Application.Transactions.ListTransactions;
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
    private readonly IListTransactionsHandler _listTransactions;

    public TransactionsController(
        ICreateTransactionHandler createTransaction,
        IGetTransactionHandler getTransaction,
        IListTransactionsHandler listTransactions)
    {
        _createTransaction = createTransaction;
        _getTransaction = getTransaction;
        _listTransactions = listTransactions;
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

    [HttpGet]
    public async Task<IActionResult> ListTransactions(string accountNumber, CancellationToken cancellationToken)
    {
        var response = await _listTransactions.HandleAsync(
            new ListTransactionsQuery(accountNumber, CallerUser.RequireId(User)),
            cancellationToken);
        return Ok(response);
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
