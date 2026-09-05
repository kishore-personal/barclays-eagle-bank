using FluentValidation;

namespace EagleBank.Application.Transactions.ListTransactions;

public sealed class ListTransactionsQueryValidator : AbstractValidator<ListTransactionsQuery>
{
    public ListTransactionsQueryValidator()
    {
        RuleFor(query => query.AccountNumber)
            .NotEmpty()
            .Matches(@"^01\d{6}$");
    }
}
