using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Transactions.GetTransaction;

public sealed class GetTransactionHandler : IGetTransactionHandler
{
    private readonly IAccountStore _accounts;
    private readonly ITransactionStore _transactions;
    private readonly IValidator<GetTransactionQuery> _validator;
    private readonly ILogger<GetTransactionHandler> _logger;

    public GetTransactionHandler(
        IAccountStore accounts,
        ITransactionStore transactions,
        IValidator<GetTransactionQuery> validator,
        ILogger<GetTransactionHandler> logger)
    {
        _accounts = accounts;
        _transactions = transactions;
        _validator = validator;
        _logger = logger;
    }

    public async Task<TransactionResponse> HandleAsync(
        GetTransactionQuery query,
        CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(GetTransaction), query.CallerUserId);
        var validation = await _validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(GetTransaction), "ValidationFailed");
            throw new ValidationException(validation.Errors);
        }

        var account = await _accounts.GetByAccountNumberAsync(query.AccountNumber, cancellationToken);
        try
        {
            ResourceOwnership.RequireOwned(account, query.CallerUserId, candidate => candidate.UserId);
        }
        catch (NotFoundException)
        {
            UseCaseLog.Completed(_logger, nameof(GetTransaction), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(GetTransaction), "Forbidden");
            throw;
        }

        var transaction = await _transactions.GetByIdAsync(query.TransactionId, cancellationToken);
        if (transaction is null
            || !string.Equals(transaction.AccountNumber, query.AccountNumber, StringComparison.Ordinal))
        {
            UseCaseLog.Completed(_logger, nameof(GetTransaction), "NotFound");
            throw new NotFoundException();
        }

        UseCaseLog.Completed(_logger, nameof(GetTransaction), "Fetched");
        return TransactionResponseMapper.ToResponse(transaction);
    }
}
