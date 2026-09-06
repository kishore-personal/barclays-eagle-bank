namespace EagleBank.Application.Transactions.CreateTransaction;

public sealed class CreateTransactionRequest
{
    public decimal? Amount { get; set; }

    public string? Currency { get; set; }

    public string? Type { get; set; }

    public string? Reference { get; set; }
}
