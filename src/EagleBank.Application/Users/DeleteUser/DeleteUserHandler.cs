using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Users.DeleteUser;

public sealed class DeleteUserHandler : IDeleteUserHandler
{
    private readonly IUserStore _users;
    private readonly IAccountStore _accounts;
    private readonly IValidator<DeleteUserCommand> _validator;
    private readonly ILogger<DeleteUserHandler> _logger;

    public DeleteUserHandler(
        IUserStore users,
        IAccountStore accounts,
        IValidator<DeleteUserCommand> validator,
        ILogger<DeleteUserHandler> logger)
    {
        _users = users;
        _accounts = accounts;
        _validator = validator;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(DeleteUser), command.CallerUserId);
        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(DeleteUser), "ValidationFailed");
            throw new ValidationException(validation.Errors);
        }

        var user = await _users.GetByIdAsync(command.UserId, cancellationToken);
        try
        {
            ResourceOwnership.RequireOwned(user, command.CallerUserId, candidate => candidate.Id);
        }
        catch (NotFoundException)
        {
            UseCaseLog.Completed(_logger, nameof(DeleteUser), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(DeleteUser), "Forbidden");
            throw;
        }

        if (await _accounts.HasAnyForUserAsync(command.UserId, cancellationToken))
        {
            UseCaseLog.Completed(_logger, nameof(DeleteUser), "Conflict");
            throw new ConflictException();
        }

        await _users.DeleteAsync(command.UserId, cancellationToken);
        UseCaseLog.Completed(_logger, nameof(DeleteUser), "Deleted");
    }
}
