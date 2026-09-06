namespace EagleBank.Application.Accounts.UpdateAccount;

public interface IUpdateAccountHandler
{
    Task<BankAccountResponse> HandleAsync(UpdateAccountCommand command, CancellationToken cancellationToken);
}
