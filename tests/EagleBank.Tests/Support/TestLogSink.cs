using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace EagleBank.Tests.Support;

public sealed class TestLogSink : ILoggerProvider
{
    private static readonly HashSet<string> AllowedScopeKeys = new(StringComparer.Ordinal)
    {
        "RequestId",
        "userId"
    };

    private readonly AsyncLocal<ImmutableList<string>> _scopes = new();

    public ConcurrentQueue<string> Entries { get; } = new();

    public ILogger CreateLogger(string categoryName) => new SinkLogger(this, categoryName);

    public void Dispose()
    {
    }

    internal IDisposable PushScope(string text)
    {
        var previous = _scopes.Value ?? ImmutableList<string>.Empty;
        _scopes.Value = previous.Add(text);
        return new PopScope(() => _scopes.Value = previous);
    }

    internal string CurrentScopeText =>
        _scopes.Value is { Count: > 0 } scopes
            ? string.Join(" ", scopes)
            : string.Empty;

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
            var text = FormatAllowedScope(state);
            return string.IsNullOrEmpty(text)
                ? NullScope.Instance
                : _sink.PushScope(text);
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
            var scope = _sink.CurrentScopeText;
            var scopePrefix = string.IsNullOrEmpty(scope) ? string.Empty : $" [{scope}]";
            _sink.Entries.Enqueue($"{logLevel} {_categoryName}{scopePrefix} {message}");
        }

        private static string FormatAllowedScope<TState>(TState state)
        {
            if (state is not IEnumerable<KeyValuePair<string, object?>> pairs)
            {
                return string.Empty;
            }

            return string.Join(
                " ",
                pairs
                    .Where(pair => AllowedScopeKeys.Contains(pair.Key) && pair.Value is not null)
                    .Select(pair => $"{pair.Key}={pair.Value}"));
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }

    private sealed class PopScope : IDisposable
    {
        private readonly Action _pop;

        public PopScope(Action pop)
        {
            _pop = pop;
        }

        public void Dispose() => _pop();
    }
}
