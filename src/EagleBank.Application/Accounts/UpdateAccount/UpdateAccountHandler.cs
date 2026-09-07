using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Accounts.UpdateAccount;

public sealed class UpdateAccountHandler : IUpdateAccountHandler
{
    private readonly IAccountStore _accounts;
    private readonly IValidator<UpdateAccountCommand> _validator;
    private readonly ILogger<UpdateAccountHandler> _logger;

    public UpdateAccountHandler(
        IAccountStore accounts,
        IValidator<UpdateAccountCommand> validator,
        ILogger<UpdateAccountHandler> logger)
    {
        _accounts = accounts;
        _validator = validator;
        _logger = logger;
    }

    public async Task<BankAccountResponse> HandleAsync(
        UpdateAccountCommand command,
        CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(UpdateAccount), command.CallerUserId);
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(UpdateAccount), "ValidationFailed");
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
            UseCaseLog.Completed(_logger, nameof(UpdateAccount), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(UpdateAccount), "Forbidden");
            throw;
        }

        owned.ApplyPartialUpdate(command.Request.Name, command.Request.AccountType, DateTimeOffset.UtcNow);
        await _accounts.UpdateAsync(owned, cancellationToken);
        UseCaseLog.Completed(_logger, nameof(UpdateAccount), "Updated");
        return BankAccountResponseMapper.ToResponse(owned);
    }
}
