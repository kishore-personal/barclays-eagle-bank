using EagleBank.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EagleBank.Tests.Support;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string TestJwtSigningKey = "TEST-ONLY-NOT-A-PRODUCTION-SECRET-32CHARS";

    private readonly string _databasePath =
        Path.Combine(Path.GetTempPath(), $"eagle-bank-tests-{Guid.NewGuid():N}.db");

    public TestLogSink LogSink { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("SkipMigrations", "true");
        builder.UseSetting("Jwt:SigningKey", TestJwtSigningKey);
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddProvider(LogSink);
        });
        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services
                         .Where(service => service.ServiceType == typeof(DbContextOptions<EagleBankDbContext>)
                                           || service.ServiceType == typeof(EagleBankDbContext))
                         .ToList())
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<EagleBankDbContext>(options =>
            {
                options.UseSqlite($"Data Source={_databasePath}");
                options.ConfigureWarnings(warnings =>
                    warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EagleBankDbContext>();
        db.Database.Migrate();
        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        SqliteConnection.ClearAllPools();
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
