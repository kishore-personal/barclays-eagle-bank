namespace EagleBank.Tests.Support;

public static class LogDenylist
{
    public static readonly string[] Values =
    [
        "password",
        "passwordHash",
        "Authorization",
        "Bearer "
    ];
}
