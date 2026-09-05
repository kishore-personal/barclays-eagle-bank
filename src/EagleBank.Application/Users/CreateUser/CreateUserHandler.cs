using EagleBank.Application.Abstractions;
using EagleBank.Application.Logging;
using EagleBank.Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Users.CreateUser;

public sealed class CreateUserHandler : ICreateUserHandler
{
    private readonly IUserStore _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserIdFactory _userIds;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(
        IUserStore users,
        IPasswordHasher passwordHasher,
        IUserIdFactory userIds,
        ILogger<CreateUserHandler> logger)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _userIds = userIds;
        _logger = logger;
    }

    public async Task<UserResponse> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(CreateUser));
        var request = command.Request;
        var email = request.Email!.Trim().ToLowerInvariant();

        if (await _users.GetByEmailAsync(email, cancellationToken) is not null)
        {
            UseCaseLog.Completed(_logger, nameof(CreateUser), "ValidationFailed");
            throw new ValidationException([
                new ValidationFailure("email", "Email is already registered.") { ErrorCode = "Duplicate" }
            ]);
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User(
            _userIds.Next(),
            request.Name!,
            new Address(
                request.Address!.Line1!,
                request.Address.Line2,
                request.Address.Line3,
                request.Address.Town!,
                request.Address.County!,
                request.Address.Postcode!),
            request.PhoneNumber!,
            email,
            _passwordHasher.Hash(request.Password!),
            now,
            now);

        await _users.AddAsync(user, cancellationToken);
        UseCaseLog.Completed(_logger, nameof(CreateUser), "Created");
        return UserResponseMapper.ToResponse(user);
    }
}
