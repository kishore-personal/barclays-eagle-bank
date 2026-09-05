using EagleBank.Domain;
using EagleBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace EagleBank.Infrastructure.Persistence.Migrations;

[DbContext(typeof(EagleBankDbContext))]
public class EagleBankDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.11");
        BuildEagleBankModel(modelBuilder);
    }

    internal static void BuildEagleBankModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(64);
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.Property(e => e.CreatedTimestamp).HasColumnName("created_timestamp");
            entity.Property(e => e.UpdatedTimestamp).HasColumnName("updated_timestamp");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.OwnsOne(e => e.Address, address =>
            {
                address.Property(v => v.Line1).HasColumnName("address_line1").IsRequired();
                address.Property(v => v.Line2).HasColumnName("address_line2");
                address.Property(v => v.Line3).HasColumnName("address_line3");
                address.Property(v => v.Town).HasColumnName("address_town").IsRequired();
                address.Property(v => v.County).HasColumnName("address_county").IsRequired();
                address.Property(v => v.Postcode).HasColumnName("address_postcode").IsRequired();
            });
        });

        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasKey(e => e.AccountNumber);
            entity.Property(e => e.AccountNumber).HasColumnName("account_number").HasMaxLength(8);
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.AccountType).HasColumnName("account_type").IsRequired();
            entity.Property(e => e.SortCode).HasColumnName("sort_code").IsRequired();
            entity.Property(e => e.Currency).HasColumnName("currency").IsRequired();
            entity.Property(e => e.CreatedTimestamp).HasColumnName("created_timestamp");
            entity.Property(e => e.UpdatedTimestamp).HasColumnName("updated_timestamp");
            entity.Property(e => e.Balance)
                .HasColumnName("balance_pence")
                .HasColumnType("INTEGER")
                .HasConversion(money => money.Pence, pence => Money.FromPence(pence));
            entity.HasIndex(e => e.UserId);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(64);
            entity.Property(e => e.AccountNumber).HasColumnName("account_number").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Currency).HasColumnName("currency").IsRequired();
            entity.Property(e => e.Reference).HasColumnName("reference");
            entity.Property(e => e.CreatedTimestamp).HasColumnName("created_timestamp");
            entity.Property(e => e.Amount)
                .HasColumnName("amount_pence")
                .HasColumnType("INTEGER")
                .HasConversion(money => money.Pence, pence => Money.FromPence(pence));
            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion(
                    type => type == TransactionType.Deposit ? "deposit" : "withdrawal",
                    stored => stored == "deposit" ? TransactionType.Deposit : TransactionType.Withdrawal);
            entity.HasIndex(e => e.AccountNumber);
            entity.HasOne(e => e.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(e => e.AccountNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
