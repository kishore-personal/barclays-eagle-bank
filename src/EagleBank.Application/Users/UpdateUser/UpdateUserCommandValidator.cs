using FluentValidation;

namespace EagleBank.Application.Users.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .Matches(@"^usr-[A-Za-z0-9]+$");
        RuleFor(command => command.Request.Name)
            .NotEmpty()
            .When(command => command.Request.Name is not null);
        RuleFor(command => command.Request.PhoneNumber)
            .Matches(@"^\+[1-9]\d{1,14}$")
            .When(command => command.Request.PhoneNumber is not null);
        RuleFor(command => command.Request.Email)
            .EmailAddress()
            .When(command => command.Request.Email is not null);
        When(command => command.Request.Address is not null, () =>
        {
            RuleFor(command => command.Request.Address!.Line1).NotEmpty();
            RuleFor(command => command.Request.Address!.Town).NotEmpty();
            RuleFor(command => command.Request.Address!.County).NotEmpty();
            RuleFor(command => command.Request.Address!.Postcode).NotEmpty();
        });
    }
}
