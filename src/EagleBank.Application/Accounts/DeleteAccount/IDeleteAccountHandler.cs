namespace EagleBank.Application.Accounts.DeleteAccount;

public interface IDeleteAccountHandler
{
    Task HandleAsync(DeleteAccountCommand command, CancellationToken cancellationToken);
}
