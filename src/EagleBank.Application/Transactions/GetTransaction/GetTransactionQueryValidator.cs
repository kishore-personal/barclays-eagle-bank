using FluentValidation;

namespace EagleBank.Application.Transactions.GetTransaction;

public sealed class GetTransactionQueryValidator : AbstractValidator<GetTransactionQuery>
{
    public GetTransactionQueryValidator()
    {
        RuleFor(query => query.AccountNumber)
            .NotEmpty()
            .Matches(@"^01\d{6}$");
        RuleFor(query => query.TransactionId)
            .NotEmpty()
            .Matches(@"^tan-[A-Za-z0-9]+$");
    }
}
