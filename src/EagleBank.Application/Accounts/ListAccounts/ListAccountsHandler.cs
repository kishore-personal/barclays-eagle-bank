using EagleBank.Application.Abstractions;
using EagleBank.Application.Logging;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Accounts.ListAccounts;

public sealed class ListAccountsHandler : IListAccountsHandler
{
    private readonly IAccountStore _accounts;
    private readonly ILogger<ListAccountsHandler> _logger;

    public ListAccountsHandler(IAccountStore accounts, ILogger<ListAccountsHandler> logger)
    {
        _accounts = accounts;
        _logger = logger;
    }

    public async Task<ListBankAccountsResponse> HandleAsync(
        ListAccountsQuery query,
        CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(ListAccounts), query.CallerUserId);
        var accounts = await _accounts.ListByUserIdAsync(query.CallerUserId, cancellationToken);
        UseCaseLog.Completed(_logger, nameof(ListAccounts), "Listed");
        return new ListBankAccountsResponse(accounts.Select(BankAccountResponseMapper.ToResponse).ToArray());
    }
}
