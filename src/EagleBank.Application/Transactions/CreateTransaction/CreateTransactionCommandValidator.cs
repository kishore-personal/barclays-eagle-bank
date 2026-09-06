using FluentValidation;

namespace EagleBank.Application.Transactions.CreateTransaction;

public sealed class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(command => command.AccountNumber)
            .NotEmpty()
            .Matches(@"^01\d{6}$");
        RuleFor(command => command.Request).SetValidator(new CreateTransactionRequestValidator());
    }
}
