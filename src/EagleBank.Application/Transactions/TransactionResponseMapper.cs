using EagleBank.Domain;

namespace EagleBank.Application.Transactions;

public static class TransactionResponseMapper
{
    public static TransactionResponse ToResponse(Transaction transaction)
    {
        return new TransactionResponse(
            transaction.Id,
            transaction.Amount.Amount,
            transaction.Currency,
            transaction.Type == TransactionType.Deposit ? "deposit" : "withdrawal",
            transaction.CreatedTimestamp,
            transaction.UserId,
            string.IsNullOrWhiteSpace(transaction.Reference) ? null : transaction.Reference);
    }
}
