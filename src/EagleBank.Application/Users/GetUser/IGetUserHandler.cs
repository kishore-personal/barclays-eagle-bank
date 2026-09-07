namespace EagleBank.Application.Users.GetUser;

public interface IGetUserHandler
{
    Task<UserResponse> HandleAsync(GetUserQuery query, CancellationToken cancellationToken);
}
