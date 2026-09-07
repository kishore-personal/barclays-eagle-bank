using EagleBank.Domain;
using EagleBank.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EagleBank.Infrastructure.Persistence;

public sealed class EagleBankDbContext : DbContext
{
    private readonly ILogger<EagleBankDbContext> _logger;

    public EagleBankDbContext(
        DbContextOptions<EagleBankDbContext> options,
        ILogger<EagleBankDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<BankAccount> Accounts => Set<BankAccount>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging(EfLogging.SensitiveDataLoggingEnabled);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EagleBankDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entityTypes = ChangeTracker.Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(entry => entry.Metadata.ClrType.Name)
            .Distinct()
            .ToArray();

        try
        {
            var rowsAffected = await base.SaveChangesAsync(cancellationToken);
            foreach (var entityType in entityTypes)
            {
                PersistenceLog.Committed(_logger, entityType, rowsAffected);
            }

            return rowsAffected;
        }
        catch
        {
            foreach (var entityType in entityTypes)
            {
                PersistenceLog.RolledBack(_logger, entityType);
            }

            throw;
        }
    }
}
