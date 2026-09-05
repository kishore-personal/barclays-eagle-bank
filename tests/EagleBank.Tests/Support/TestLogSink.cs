using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace EagleBank.Tests.Support;

public sealed class TestLogSink : ILoggerProvider
{
    public ConcurrentQueue<string> Entries { get; } = new();

    public ILogger CreateLogger(string categoryName) => new SinkLogger(this, categoryName);

    public void Dispose()
    {
    }

    private sealed class SinkLogger : ILogger
    {
        private readonly TestLogSink _sink;
        private readonly string _categoryName;

        public SinkLogger(TestLogSink sink, string categoryName)
        {
            _sink = sink;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            _sink.Entries.Enqueue($"{logLevel} {_categoryName} {message}");
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
