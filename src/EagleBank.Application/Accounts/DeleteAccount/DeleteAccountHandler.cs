using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Accounts.DeleteAccount;

public sealed class DeleteAccountHandler : IDeleteAccountHandler
{
    private readonly IAccountStore _accounts;
    private readonly IValidator<DeleteAccountCommand> _validator;
    private readonly ILogger<DeleteAccountHandler> _logger;

    public DeleteAccountHandler(
        IAccountStore accounts,
        IValidator<DeleteAccountCommand> validator,
        ILogger<DeleteAccountHandler> logger)
    {
        _accounts = accounts;
        _validator = validator;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(DeleteAccount), command.CallerUserId);
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(DeleteAccount), "ValidationFailed");
            throw new ValidationException(validation.Errors);
        }

        var account = await _accounts.GetByAccountNumberAsync(command.AccountNumber, cancellationToken);
        try
        {
            ResourceOwnership.RequireOwned(account, command.CallerUserId, candidate => candidate.UserId);
        }
        catch (NotFoundException)
        {
            UseCaseLog.Completed(_logger, nameof(DeleteAccount), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(DeleteAccount), "Forbidden");
            throw;
        }

        await _accounts.DeleteAsync(command.AccountNumber, cancellationToken);
        UseCaseLog.Completed(_logger, nameof(DeleteAccount), "Deleted");
    }
}
