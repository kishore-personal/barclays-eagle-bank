using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain;
using EagleBank.Domain.Exceptions;
using EagleBank.Domain.Logging;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Transactions.CreateTransaction;

public sealed class CreateTransactionHandler : ICreateTransactionHandler
{
    private readonly IAccountStore _accounts;
    private readonly ITransactionStore _transactions;
    private readonly ITransactionIdFactory _transactionIds;
    private readonly IValidator<CreateTransactionCommand> _validator;
    private readonly ILogger<CreateTransactionHandler> _logger;

    public CreateTransactionHandler(
        IAccountStore accounts,
        ITransactionStore transactions,
        ITransactionIdFactory transactionIds,
        IValidator<CreateTransactionCommand> validator,
        ILogger<CreateTransactionHandler> logger)
    {
        _accounts = accounts;
        _transactions = transactions;
        _transactionIds = transactionIds;
        _validator = validator;
        _logger = logger;
    }

    public async Task<TransactionResponse> HandleAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(CreateTransaction), command.CallerUserId);
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(CreateTransaction), "ValidationFailed");
            throw new ValidationException(validation.Errors);
        }

        var account = await _accounts.GetByAccountNumberAsync(command.AccountNumber, cancellationToken);
        BankAccount owned;
        try
        {
            owned = ResourceOwnership.RequireOwned(account, command.CallerUserId, candidate => candidate.UserId);
        }
        catch (NotFoundException)
        {
            UseCaseLog.Completed(_logger, nameof(CreateTransaction), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(CreateTransaction), "Forbidden");
            throw;
        }

        var type = command.Request.Type == "withdrawal"
            ? TransactionType.Withdrawal
            : TransactionType.Deposit;
        var transaction = new Transaction(
            _transactionIds.Next(),
            owned.AccountNumber,
            owned.UserId,
            Money.FromMajorUnits(command.Request.Amount!.Value),
            type,
            command.Request.Reference,
            DateTimeOffset.UtcNow);

        try
        {
            owned.Apply(transaction);
            await _transactions.AddAtomicAsync(transaction, cancellationToken);
        }
        catch (BalanceCapException)
        {
            InvariantLog.Rejected(_logger, "BalanceCap");
            UseCaseLog.Completed(_logger, nameof(CreateTransaction), "BalanceCap");
            throw;
        }
        catch (InsufficientFundsException)
        {
            InvariantLog.Rejected(_logger, "InsufficientFunds");
            UseCaseLog.Completed(_logger, nameof(CreateTransaction), "InsufficientFunds");
            throw;
        }

        UseCaseLog.Completed(_logger, nameof(CreateTransaction), "Created");
        return TransactionResponseMapper.ToResponse(transaction);
    }
}
