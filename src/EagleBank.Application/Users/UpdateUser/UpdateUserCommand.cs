namespace EagleBank.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(string UserId, string CallerUserId, UpdateUserRequest Request);
