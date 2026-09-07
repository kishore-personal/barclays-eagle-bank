namespace EagleBank.Application.Users.DeleteUser;

public sealed record DeleteUserCommand(string UserId, string CallerUserId);
