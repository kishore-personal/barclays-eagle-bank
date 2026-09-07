using System.Net.Http.Json;
using System.Text.Json;

namespace EagleBank.Tests.Support;

public static class UserFixtures
{
    public const string Name = "Ada Lovelace";
    public const string Email = "ada@example.com";
    public const string OtherName = "Grace Hopper";
    public const string OtherEmail = "grace@example.com";
    public const string Phone = "+441234567890";
    public const string Password = "S3cretPass!";
    public const string Line1 = "Stoney Street";
    public const string Town = "London";
    public const string County = "Greater London";
    public const string Postcode = "SE1 9TG";

    public static object OtherCreateBody() => new
    {
        name = OtherName,
        address = new
        {
            line1 = Line1,
            town = Town,
            county = County,
            postcode = Postcode
        },
        phoneNumber = Phone,
        email = OtherEmail,
        password = Password
    };

    public static object ValidCreateBody() => new
    {
        name = Name,
        address = new
        {
            line1 = Line1,
            town = Town,
            county = County,
            postcode = Postcode
        },
        phoneNumber = Phone,
        email = Email,
        password = Password
    };

    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static HttpContent JsonBody(object value) =>
        JsonContent.Create(value, options: JsonOptions);
}
