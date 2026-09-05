using EagleBank.Domain;
using FluentValidation;

namespace EagleBank.Application.Transactions.CreateTransaction;

public sealed class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionRequestValidator()
    {
        RuleFor(request => request.Amount)
            .NotNull()
            .GreaterThan(0m)
            .LessThanOrEqualTo(10000.00m)
            .Must(amount => amount is null || decimal.Round(amount.Value, Money.Scale) == amount.Value)
            .WithMessage("Amount must have at most two decimal places.");
        RuleFor(request => request.Currency).NotEmpty().Equal(Money.GbpCurrency);
        RuleFor(request => request.Type)
            .NotEmpty()
            .Must(type => type is "deposit" or "withdrawal")
            .WithMessage("Type must be deposit or withdrawal.");
    }
}
