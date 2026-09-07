using FluentValidation;

namespace EagleBank.Application.Users.CreateUser;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{1,14}$");
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Password).NotEmpty().MinimumLength(8);
        RuleFor(request => request.Address).NotNull();
        When(request => request.Address is not null, () =>
        {
            RuleFor(request => request.Address!.Line1).NotEmpty();
            RuleFor(request => request.Address!.Town).NotEmpty();
            RuleFor(request => request.Address!.County).NotEmpty();
            RuleFor(request => request.Address!.Postcode).NotEmpty();
        });
    }
}
