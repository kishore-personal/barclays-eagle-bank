using FluentValidation;

namespace EagleBank.Application.Users.DeleteUser;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .Matches(@"^usr-[A-Za-z0-9]+$");
    }
}
