namespace EagleBank.Application.Users.DeleteUser;

public interface IDeleteUserHandler
{
    Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken);
}
