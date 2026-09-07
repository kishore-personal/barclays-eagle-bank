namespace EagleBank.Application.Abstractions;

public interface IJwtTokenIssuer
{
    string Issue(string userId);
}
