using EagleBank.Domain;
using FluentValidation;

namespace EagleBank.Application.Accounts.UpdateAccount;

public sealed class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(command => command.AccountNumber)
            .NotEmpty()
            .Matches(@"^01\d{6}$");
        RuleFor(command => command.Request.Name)
            .NotEmpty()
            .When(command => command.Request.Name is not null);
        RuleFor(command => command.Request.AccountType)
            .Equal(BankAccount.PersonalAccountType)
            .When(command => command.Request.AccountType is not null);
    }
}
