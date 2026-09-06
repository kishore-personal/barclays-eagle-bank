using EagleBank.Domain;

namespace EagleBank.Application.Users;

public static class UserResponseMapper
{
    public static UserResponse ToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            new UserAddressResponse(
                user.Address.Line1,
                user.Address.Line2,
                user.Address.Line3,
                user.Address.Town,
                user.Address.County,
                user.Address.Postcode),
            user.PhoneNumber,
            user.Email,
            user.CreatedTimestamp,
            user.UpdatedTimestamp);
    }
}
