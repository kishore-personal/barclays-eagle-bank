using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Accounts.GetAccount;

public sealed class GetAccountHandler : IGetAccountHandler
{
    private readonly IAccountStore _accounts;
    private readonly IValidator<GetAccountQuery> _validator;
    private readonly ILogger<GetAccountHandler> _logger;

    public GetAccountHandler(
        IAccountStore accounts,
        IValidator<GetAccountQuery> validator,
        ILogger<GetAccountHandler> logger)
    {
        _accounts = accounts;
        _validator = validator;
        _logger = logger;
    }

    public async Task<BankAccountResponse> HandleAsync(
        GetAccountQuery query,
        CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(GetAccount), query.CallerUserId);
        var validation = await _validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(GetAccount), "ValidationFailed");
            throw new ValidationException(validation.Errors);
        }

        var account = await _accounts.GetByAccountNumberAsync(query.AccountNumber, cancellationToken);
        try
        {
            var owned = ResourceOwnership.RequireOwned(account, query.CallerUserId, candidate => candidate.UserId);
            UseCaseLog.Completed(_logger, nameof(GetAccount), "Fetched");
            return BankAccountResponseMapper.ToResponse(owned);
        }
        catch (NotFoundException)
        {
            UseCaseLog.Completed(_logger, nameof(GetAccount), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(GetAccount), "Forbidden");
            throw;
        }
    }
}
