using EagleBank.Application.Abstractions;
using EagleBank.Infrastructure.Identity;
using EagleBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EagleBank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EagleBank")
            ?? "Data Source=data/eagle-bank.db";

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddDbContext<EagleBankDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        services.AddScoped<IUserStore, UserStore>();
        services.AddScoped<IAccountStore, AccountStore>();
        services.AddSingleton<AccountNumberFactory>();
        services.AddSingleton<IPasswordHasher, AspNetPasswordHasher>();
        services.AddSingleton<IUserIdFactory, UserIdFactory>();
        services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();
        return services;
    }
}
