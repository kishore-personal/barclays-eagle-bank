using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Transactions.ListTransactions;

public sealed class ListTransactionsHandler : IListTransactionsHandler
{
    private readonly IAccountStore _accounts;
    private readonly ITransactionStore _transactions;
    private readonly IValidator<ListTransactionsQuery> _validator;
    private readonly ILogger<ListTransactionsHandler> _logger;

    public ListTransactionsHandler(
        IAccountStore accounts,
        ITransactionStore transactions,
        IValidator<ListTransactionsQuery> validator,
        ILogger<ListTransactionsHandler> logger)
    {
        _accounts = accounts;
        _transactions = transactions;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ListTransactionsResponse> HandleAsync(
        ListTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(ListTransactions), query.CallerUserId);
        var validation = await _validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(ListTransactions), "ValidationFailed");
            throw new ValidationException(validation.Errors);
        }

        var account = await _accounts.GetByAccountNumberAsync(query.AccountNumber, cancellationToken);
        try
        {
            ResourceOwnership.RequireOwned(account, query.CallerUserId, candidate => candidate.UserId);
        }
        catch (NotFoundException)
        {
            UseCaseLog.Completed(_logger, nameof(ListTransactions), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(ListTransactions), "Forbidden");
            throw;
        }

        var transactions = await _transactions.ListByAccountNumberAsync(query.AccountNumber, cancellationToken);
        UseCaseLog.Completed(_logger, nameof(ListTransactions), "Listed");
        return new ListTransactionsResponse(transactions.Select(TransactionResponseMapper.ToResponse).ToArray());
    }
}
