using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace EagleBank.Infrastructure.Persistence;

public sealed class EagleBankDbContextFactory : IDesignTimeDbContextFactory<EagleBankDbContext>
{
    public EagleBankDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EagleBankDbContext>()
            .UseSqlite("Data Source=data/eagle-bank.db")
            .ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        return new EagleBankDbContext(options, NullLogger<EagleBankDbContext>.Instance);
    }
}
