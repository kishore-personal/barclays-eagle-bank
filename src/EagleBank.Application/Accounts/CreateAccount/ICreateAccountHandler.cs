namespace EagleBank.Application.Accounts.CreateAccount;

public interface ICreateAccountHandler
{
    Task<BankAccountResponse> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken);
}
