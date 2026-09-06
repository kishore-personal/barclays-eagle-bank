namespace EagleBank.Application.Auth.Login;

public sealed class LoginRequest
{
    public string? Email { get; set; }

    public string? Password { get; set; }
}
