using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EagleBank.Application.Abstractions;
using EagleBank.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EagleBank.Infrastructure.Identity;

public sealed class JwtTokenIssuer : IJwtTokenIssuer
{
    private readonly JwtOptions _options;
    private readonly ILogger<JwtTokenIssuer> _logger;

    public JwtTokenIssuer(IOptions<JwtOptions> options, ILogger<JwtTokenIssuer> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string Issue(string userId)
    {
        if (_options.SigningKey.Length < 32)
        {
            throw new InvalidOperationException("JWT signing key is not configured.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            [new Claim(JwtRegisteredClaimNames.Sub, userId)],
            expires: DateTime.UtcNow.AddMinutes(_options.LifetimeMinutes),
            signingCredentials: credentials);

        var encoded = new JwtSecurityTokenHandler().WriteToken(token);
        JwtIssueLog.Issued(_logger);
        return encoded;
    }
}
