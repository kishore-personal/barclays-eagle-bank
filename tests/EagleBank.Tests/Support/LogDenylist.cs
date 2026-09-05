namespace EagleBank.Tests.Support;

public static class LogDenylist
{
    public static readonly string[] Values =
    [
        "passwordHash",
        "Authorization",
        "Bearer ",
        UserFixtures.Email,
        UserFixtures.OtherEmail,
        UserFixtures.OtherName,
        UserFixtures.Phone,
        UserFixtures.Password,
        UserFixtures.Line1,
        UserFixtures.Name,
        AccountFixtures.Name,
        "unknown@example.com",
        "eyJ"
    ];
}
