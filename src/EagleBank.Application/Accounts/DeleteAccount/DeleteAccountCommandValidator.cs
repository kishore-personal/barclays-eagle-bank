using FluentValidation;

namespace EagleBank.Application.Accounts.DeleteAccount;

public sealed class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(command => command.AccountNumber)
            .NotEmpty()
            .Matches(@"^01\d{6}$");
    }
}
