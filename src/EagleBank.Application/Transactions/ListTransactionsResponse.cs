namespace EagleBank.Application.Transactions;

public sealed record ListTransactionsResponse(IReadOnlyList<TransactionResponse> Transactions);
