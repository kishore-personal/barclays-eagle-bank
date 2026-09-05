namespace EagleBank.Application.Auth.Login;

public interface ILoginHandler
{
    Task<LoginResponse> HandleAsync(LoginCommand command, CancellationToken cancellationToken);
}
