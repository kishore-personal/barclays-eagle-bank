namespace EagleBank.Application.Users.UpdateUser;

public sealed class UpdateUserRequest
{
    public string? Name { get; set; }

    public UpdateAddressDto? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }
}

public sealed class UpdateAddressDto
{
    public string? Line1 { get; set; }

    public string? Line2 { get; set; }

    public string? Line3 { get; set; }

    public string? Town { get; set; }

    public string? County { get; set; }

    public string? Postcode { get; set; }
}
