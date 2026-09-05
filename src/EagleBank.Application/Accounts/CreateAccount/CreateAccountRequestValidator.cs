using EagleBank.Domain;
using FluentValidation;

namespace EagleBank.Application.Accounts.CreateAccount;

public sealed class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.AccountType)
            .NotEmpty()
            .Equal(BankAccount.PersonalAccountType);
    }
}
