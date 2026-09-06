using EagleBank.Application.Abstractions;
using EagleBank.Application.Logging;
using EagleBank.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Auth.Login;

public sealed class LoginHandler : ILoginHandler
{
    private readonly IUserStore _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenIssuer _tokens;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        IUserStore users,
        IPasswordHasher passwordHasher,
        IJwtTokenIssuer tokens,
        ILogger<LoginHandler> logger)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokens = tokens;
        _logger = logger;
    }

    public async Task<LoginResponse> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(Login));
        var email = command.Request.Email!.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(user.PasswordHash, command.Request.Password!))
        {
            UseCaseLog.Completed(_logger, nameof(Login), "Unauthorized");
            throw new UnauthorizedException();
        }

        var token = _tokens.Issue(user.Id);
        UseCaseLog.Completed(_logger, nameof(Login), "Authenticated");
        return new LoginResponse(token);
    }
}
