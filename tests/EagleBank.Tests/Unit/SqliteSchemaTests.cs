using EagleBank.Domain;
using EagleBank.Infrastructure.Logging;
using EagleBank.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace EagleBank.Tests.Unit;

public class SqliteSchemaTests
{
    [Fact]
    public async Task Migrate_creates_integer_pence_columns()
    {
        var path = Path.Combine(Path.GetTempPath(), $"eagle-bank-schema-{Guid.NewGuid():N}.db");

        try
        {
            var options = new DbContextOptionsBuilder<EagleBankDbContext>()
                .UseSqlite($"Data Source={path}")
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
                .Options;

            await using var context = new EagleBankDbContext(options, NullLogger<EagleBankDbContext>.Instance);
            await context.Database.MigrateAsync();

            var columns = await LoadColumnsAsync(context, "accounts");
            Assert.Equal("INTEGER", columns["balance_pence"]);
            Assert.DoesNotContain("REAL", columns["balance_pence"], StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("DOUBLE", columns["balance_pence"], StringComparison.OrdinalIgnoreCase);

            var transactionColumns = await LoadColumnsAsync(context, "transactions");
            Assert.Equal("INTEGER", transactionColumns["amount_pence"]);

            var userColumns = await LoadColumnsAsync(context, "users");
            Assert.Contains("email", userColumns.Keys);
            Assert.Contains("password_hash", userColumns.Keys);
            Assert.Contains("address_line1", userColumns.Keys);

            await PersistSampleAsync(context);
            Assert.False(EfLogging.SensitiveDataLoggingEnabled);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static async Task PersistSampleAsync(EagleBankDbContext context)
    {
        var now = DateTimeOffset.UtcNow;
        var user = new User(
            "usr-schema1",
            "Schema User",
            new Address("1 Road", null, null, "Town", "County", "AB1 2CD"),
            "+441234567890",
            "schema@example.com",
            "hashed",
            now,
            now);
        var account = new BankAccount("01234567", user.Id, "Everyday", now);
        var transaction = new Transaction(
            "tan-abc123",
            account.AccountNumber,
            user.Id,
            Money.FromMajorUnits(10.99m),
            TransactionType.Deposit,
            null,
            now);

        context.Users.Add(user);
        context.Accounts.Add(account);
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();
        var stored = await context.Accounts.SingleAsync(a => a.AccountNumber == "01234567");
        Assert.Equal(0, stored.Balance.Pence);

        var storedTxn = await context.Transactions.SingleAsync(t => t.Id == "tan-abc123");
        Assert.Equal(1099, storedTxn.Amount.Pence);
        Assert.Equal(TransactionType.Deposit, storedTxn.Type);
    }

    private static async Task<Dictionary<string, string>> LoadColumnsAsync(EagleBankDbContext context, string table)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({table});";
        var columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            columns[reader.GetString(1)] = reader.GetString(2);
        }

        return columns;
    }
}
