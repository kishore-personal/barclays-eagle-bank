using EagleBank.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EagleBank.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).HasColumnName("id").HasMaxLength(64);
        builder.Property(user => user.Name).HasColumnName("name").IsRequired();
        builder.Property(user => user.PhoneNumber).HasColumnName("phone_number").IsRequired();
        builder.Property(user => user.Email).HasColumnName("email").IsRequired();
        builder.Property(user => user.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(user => user.CreatedTimestamp).HasColumnName("created_timestamp");
        builder.Property(user => user.UpdatedTimestamp).HasColumnName("updated_timestamp");
        builder.HasIndex(user => user.Email).IsUnique();

        builder.OwnsOne(user => user.Address, address =>
        {
            address.Property(value => value.Line1).HasColumnName("address_line1").IsRequired();
            address.Property(value => value.Line2).HasColumnName("address_line2");
            address.Property(value => value.Line3).HasColumnName("address_line3");
            address.Property(value => value.Town).HasColumnName("address_town").IsRequired();
            address.Property(value => value.County).HasColumnName("address_county").IsRequired();
            address.Property(value => value.Postcode).HasColumnName("address_postcode").IsRequired();
        });

        builder.HasMany(user => user.Accounts)
            .WithOne(account => account.User)
            .HasForeignKey(account => account.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
