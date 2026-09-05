namespace EagleBank.Application.Users;

public sealed record UserResponse(
    string Id,
    string Name,
    UserAddressResponse Address,
    string PhoneNumber,
    string Email,
    DateTimeOffset CreatedTimestamp,
    DateTimeOffset UpdatedTimestamp);

public sealed record UserAddressResponse(
    string Line1,
    string? Line2,
    string? Line3,
    string Town,
    string County,
    string Postcode);
