namespace EagleBank.Application.Users.CreateUser;

public interface ICreateUserHandler
{
    Task<UserResponse> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken);
}
