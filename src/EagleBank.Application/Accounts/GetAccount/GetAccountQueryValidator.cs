using FluentValidation;

namespace EagleBank.Application.Accounts.GetAccount;

public sealed class GetAccountQueryValidator : AbstractValidator<GetAccountQuery>
{
    public GetAccountQueryValidator()
    {
        RuleFor(query => query.AccountNumber)
            .NotEmpty()
            .Matches(@"^01\d{6}$");
    }
}
