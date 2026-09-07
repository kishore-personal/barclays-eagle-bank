namespace EagleBank.Application.Users.UpdateUser;

public interface IUpdateUserHandler
{
    Task<UserResponse> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken);
}
