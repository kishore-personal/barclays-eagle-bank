namespace EagleBank.Application.Transactions.ListTransactions;

public sealed record ListTransactionsQuery(string AccountNumber, string CallerUserId);
