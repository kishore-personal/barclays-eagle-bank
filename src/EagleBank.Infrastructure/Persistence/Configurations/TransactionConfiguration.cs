using EagleBank.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EagleBank.Infrastructure.Persistence.Configurations;

public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(transaction => transaction.Id);
        builder.Property(transaction => transaction.Id).HasColumnName("id").HasMaxLength(64);
        builder.Property(transaction => transaction.AccountNumber).HasColumnName("account_number").IsRequired();
        builder.Property(transaction => transaction.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(transaction => transaction.Currency).HasColumnName("currency").IsRequired();
        builder.Property(transaction => transaction.Reference).HasColumnName("reference");
        builder.Property(transaction => transaction.CreatedTimestamp).HasColumnName("created_timestamp");
        builder.HasIndex(transaction => transaction.AccountNumber);

        builder.Property(transaction => transaction.Amount)
            .HasColumnName("amount_pence")
            .HasColumnType("INTEGER")
            .HasConversion(money => money.Pence, pence => Money.FromPence(pence))
            .IsRequired();

        builder.Property(transaction => transaction.Type)
            .HasColumnName("type")
            .HasConversion(
                type => type == TransactionType.Deposit ? "deposit" : "withdrawal",
                stored => stored == "deposit" ? TransactionType.Deposit : TransactionType.Withdrawal)
            .IsRequired();
    }
}
