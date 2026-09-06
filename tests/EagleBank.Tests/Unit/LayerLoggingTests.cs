using EagleBank.Application.Logging;
using EagleBank.Domain.Logging;
using EagleBank.Infrastructure.Logging;
using EagleBank.Tests.Support;
using Microsoft.Extensions.Logging;

namespace EagleBank.Tests.Unit;

public class LayerLoggingTests
{
    [Fact]
    public void Domain_logs_reason_code_only()
    {
        var (logger, sink) = CreateLogger();

        InvariantLog.Rejected(logger, "BalanceCap");

        var entry = Assert.Single(sink.Entries);
        Assert.Contains("BalanceCap", entry);
        Assert.DoesNotContain("10000", entry);
        Assert.DoesNotContain("email", entry, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Application_logs_use_case_and_result_code()
    {
        var (logger, sink) = CreateLogger();

        UseCaseLog.Started(logger, "CreateUser");
        UseCaseLog.Completed(logger, "CreateUser", "Created");

        Assert.Contains(sink.Entries, e => e.Contains("CreateUser") && e.Contains("started", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(sink.Entries, e => e.Contains("Created"));
        Assert.All(sink.Entries, e => Assert.DoesNotContain("password", e, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Infrastructure_logs_entity_type_not_parameters()
    {
        var (logger, sink) = CreateLogger();

        PersistenceLog.Committed(logger, "User", 1);
        PersistenceLog.RolledBack(logger, "Transaction");
        JwtIssueLog.Issued(logger);

        Assert.Contains(sink.Entries, e => e.Contains("User") && e.Contains("committed", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(sink.Entries, e => e.Contains("JWT issued"));
        Assert.All(sink.Entries, e => Assert.DoesNotContain("@email", e, StringComparison.OrdinalIgnoreCase));
        Assert.False(EfLogging.SensitiveDataLoggingEnabled);
    }

    private static (ILogger Logger, TestLogSink Sink) CreateLogger()
    {
        var sink = new TestLogSink();
        return (sink.CreateLogger("test"), sink);
    }
}
