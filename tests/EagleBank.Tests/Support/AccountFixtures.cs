namespace EagleBank.Tests.Support;

public static class AccountFixtures
{
    public const string Name = "Personal Bank Account";
    public const string AccountType = "personal";

    public static object ValidCreateBody() => new
    {
        name = Name,
        accountType = AccountType
    };
}
