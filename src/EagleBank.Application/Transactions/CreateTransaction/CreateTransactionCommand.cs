namespace EagleBank.Application.Transactions.CreateTransaction;

public sealed record CreateTransactionCommand(
    string AccountNumber,
    string CallerUserId,
    CreateTransactionRequest Request);
