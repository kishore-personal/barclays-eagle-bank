using FluentValidation;

namespace EagleBank.Application.Users.GetUser;

public sealed class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(query => query.UserId)
            .NotEmpty()
            .Matches(@"^usr-[A-Za-z0-9]+$");
    }
}
