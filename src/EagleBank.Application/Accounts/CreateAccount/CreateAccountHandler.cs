using EagleBank.Application.Abstractions;
using EagleBank.Application.Logging;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Accounts.CreateAccount;

public sealed class CreateAccountHandler : ICreateAccountHandler
{
    private readonly IAccountStore _accounts;
    private readonly ILogger<CreateAccountHandler> _logger;

    public CreateAccountHandler(IAccountStore accounts, ILogger<CreateAccountHandler> logger)
    {
        _accounts = accounts;
        _logger = logger;
    }

    public async Task<BankAccountResponse> HandleAsync(
        CreateAccountCommand command,
        CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(CreateAccount), command.CallerUserId);
        var account = await _accounts.AddAsync(command.CallerUserId, command.Request.Name!, cancellationToken);
        UseCaseLog.Completed(_logger, nameof(CreateAccount), "Created");
        return BankAccountResponseMapper.ToResponse(account);
    }
}
