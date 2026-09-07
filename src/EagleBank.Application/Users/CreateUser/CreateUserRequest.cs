namespace EagleBank.Application.Users.CreateUser;

public sealed class CreateUserRequest
{
    public string? Name { get; set; }

    public AddressDto? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
}

public sealed class AddressDto
{
    public string? Line1 { get; set; }

    public string? Line2 { get; set; }

    public string? Line3 { get; set; }

    public string? Town { get; set; }

    public string? County { get; set; }

    public string? Postcode { get; set; }
}
