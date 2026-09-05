namespace EagleBank.Application.Transactions.GetTransaction;

public sealed record GetTransactionQuery(string AccountNumber, string TransactionId, string CallerUserId);
