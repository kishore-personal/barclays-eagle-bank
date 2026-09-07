namespace EagleBank.Infrastructure.Identity;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "eagle-bank";

    public string Audience { get; set; } = "eagle-bank";

    public string SigningKey { get; set; } = string.Empty;

    public int LifetimeMinutes { get; set; } = 60;
}
