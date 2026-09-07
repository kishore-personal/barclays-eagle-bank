using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Users.UpdateUser;

public sealed class UpdateUserHandler : IUpdateUserHandler
{
    private readonly IUserStore _users;
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(
        IUserStore users,
        IValidator<UpdateUserCommand> validator,
        ILogger<UpdateUserHandler> logger)
    {
        _users = users;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UserResponse> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(UpdateUser), command.CallerUserId);
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(UpdateUser), "ValidationFailed");
            throw new ValidationException(validation.Errors);
        }

        var user = await _users.GetByIdAsync(command.UserId, cancellationToken);
        User owned;
        try
        {
            owned = ResourceOwnership.RequireOwned(user, command.CallerUserId, candidate => candidate.Id);
        }
        catch (NotFoundException)
        {
            UseCaseLog.Completed(_logger, nameof(UpdateUser), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(UpdateUser), "Forbidden");
            throw;
        }

        var email = command.Request.Email?.Trim().ToLowerInvariant();
        if (email is not null
            && !string.Equals(email, owned.Email, StringComparison.Ordinal)
            && await _users.GetByEmailAsync(email, cancellationToken) is not null)
        {
            UseCaseLog.Completed(_logger, nameof(UpdateUser), "ValidationFailed");
            throw new ValidationException([
                new ValidationFailure("email", "Email is already registered.") { ErrorCode = "Duplicate" }
            ]);
        }

        Address? address = null;
        if (command.Request.Address is not null)
        {
            address = new Address(
                command.Request.Address.Line1!,
                command.Request.Address.Line2,
                command.Request.Address.Line3,
                command.Request.Address.Town!,
                command.Request.Address.County!,
                command.Request.Address.Postcode!);
        }

        owned.ApplyPartialUpdate(
            command.Request.Name,
            address,
            command.Request.PhoneNumber,
            email,
            DateTimeOffset.UtcNow);
        await _users.UpdateAsync(owned, cancellationToken);
        UseCaseLog.Completed(_logger, nameof(UpdateUser), "Updated");
        return UserResponseMapper.ToResponse(owned);
    }
}
