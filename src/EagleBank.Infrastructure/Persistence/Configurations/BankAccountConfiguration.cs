using EagleBank.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EagleBank.Infrastructure.Persistence.Configurations;

public sealed class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(account => account.AccountNumber);
        builder.Property(account => account.AccountNumber).HasColumnName("account_number").HasMaxLength(8);
        builder.Property(account => account.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(account => account.Name).HasColumnName("name").IsRequired();
        builder.Property(account => account.AccountType).HasColumnName("account_type").IsRequired();
        builder.Property(account => account.SortCode).HasColumnName("sort_code").IsRequired();
        builder.Property(account => account.Currency).HasColumnName("currency").IsRequired();
        builder.Property(account => account.CreatedTimestamp).HasColumnName("created_timestamp");
        builder.Property(account => account.UpdatedTimestamp).HasColumnName("updated_timestamp");
        builder.HasIndex(account => account.UserId);

        builder.Property(account => account.Balance)
            .HasColumnName("balance_pence")
            .HasColumnType("INTEGER")
            .HasConversion(money => money.Pence, pence => Money.FromPence(pence))
            .IsRequired();

        builder.HasMany(account => account.Transactions)
            .WithOne(transaction => transaction.Account)
            .HasForeignKey(transaction => transaction.AccountNumber)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
