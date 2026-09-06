namespace EagleBank.Domain;

public sealed class User
{
    public string Id { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public string PhoneNumber { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public DateTimeOffset CreatedTimestamp { get; private set; }

    public DateTimeOffset UpdatedTimestamp { get; private set; }

    public ICollection<BankAccount> Accounts { get; private set; } = new List<BankAccount>();

    private User()
    {
    }

    public User(
        string id,
        string name,
        Address address,
        string phoneNumber,
        string email,
        string passwordHash,
        DateTimeOffset createdTimestamp,
        DateTimeOffset updatedTimestamp)
    {
        Id = id;
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        PasswordHash = passwordHash;
        CreatedTimestamp = createdTimestamp;
        UpdatedTimestamp = updatedTimestamp;
    }

    public void ApplyPartialUpdate(
        string? name,
        Address? address,
        string? phoneNumber,
        string? email,
        DateTimeOffset updatedTimestamp)
    {
        if (name is not null)
        {
            Name = name;
        }

        if (address is not null)
        {
            Address = address;
        }

        if (phoneNumber is not null)
        {
            PhoneNumber = phoneNumber;
        }

        if (email is not null)
        {
            Email = email;
        }

        UpdatedTimestamp = updatedTimestamp;
    }
}
