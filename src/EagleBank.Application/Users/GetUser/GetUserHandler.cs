using EagleBank.Application.Abstractions;
using EagleBank.Application.Authorization;
using EagleBank.Application.Logging;
using EagleBank.Domain.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EagleBank.Application.Users.GetUser;

public sealed class GetUserHandler : IGetUserHandler
{
    private readonly IUserStore _users;
    private readonly IValidator<GetUserQuery> _validator;
    private readonly ILogger<GetUserHandler> _logger;

    public GetUserHandler(
        IUserStore users,
        IValidator<GetUserQuery> validator,
        ILogger<GetUserHandler> logger)
    {
        _users = users;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UserResponse> HandleAsync(GetUserQuery query, CancellationToken cancellationToken)
    {
        UseCaseLog.Started(_logger, nameof(GetUser), query.CallerUserId);
        var validation = await _validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            UseCaseLog.Completed(_logger, nameof(GetUser), "ValidationFailed");
            throw new ValidationException(validation.Errors);
        }

        var user = await _users.GetByIdAsync(query.UserId, cancellationToken);
        try
        {
            var owned = ResourceOwnership.RequireOwned(user, query.CallerUserId, candidate => candidate.Id);
            UseCaseLog.Completed(_logger, nameof(GetUser), "Fetched");
            return UserResponseMapper.ToResponse(owned);
        }
        catch (NotFoundException)
        {
            UseCaseLog.Completed(_logger, nameof(GetUser), "NotFound");
            throw;
        }
        catch (ForbiddenException)
        {
            UseCaseLog.Completed(_logger, nameof(GetUser), "Forbidden");
            throw;
        }
    }
}
