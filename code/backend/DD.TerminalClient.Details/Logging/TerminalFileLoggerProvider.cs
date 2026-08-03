using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DD.TerminalClient.Details.Storage;
using Microsoft.Extensions.Logging;

namespace DD.TerminalClient.Details.Logging;

// The only logging sink in interactive mode. The alternate-screen UI owns the console, so nothing may
// be written there; diagnostics go to a bounded, per-profile file that rotates once it reaches a size
// cap, keeping a single previous file so the footprint stays roughly twice the cap. Every line is run
// through a redactor that strips JWTs, bearer headers, and password values: a defense-in-depth
// backstop so a stray log call can never persist a credential.
public sealed partial class TerminalFileLoggerProvider(
    ApplicationPathProvider paths,
    string profileName,
    TimeProvider timeProvider,
    long maxBytes = TerminalFileLoggerProvider.DefaultMaxBytes) : ILoggerProvider
{
    private const long DefaultMaxBytes = 1024 * 1024;
    private const string RedactionMarker = "***redacted***";

    private readonly string _logFilePath = EnsureLogDirectory(paths, profileName);
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly long _maxBytes = maxBytes;
    private readonly object _gate = new();

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(this, categoryName);
    }

    public void Dispose()
    {
    }

    internal static string Redact(string text)
    {
        var redacted = JwtRegex().Replace(text, RedactionMarker);
        redacted = BearerRegex().Replace(redacted, $"Bearer {RedactionMarker}");
        redacted = PasswordRegex().Replace(redacted, $"$1{RedactionMarker}$3");
        return redacted;
    }

    private static string EnsureLogDirectory(ApplicationPathProvider paths, string profileName)
    {
        var directory = Path.Combine(paths.GetProfileStateDirectory(profileName), "logs");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "terminal.log");
    }

    [GeneratedRegex(@"eyJ[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+")]
    private static partial Regex JwtRegex();

    [GeneratedRegex(@"\bBearer\s+[A-Za-z0-9._~+/=-]+", RegexOptions.IgnoreCase)]
    private static partial Regex BearerRegex();

    [GeneratedRegex("(\"?password\"?\\s*[:=]\\s*\"?)([^\"\\s,}]+)(\"?)", RegexOptions.IgnoreCase)]
    private static partial Regex PasswordRegex();

    private void Write(LogLevel logLevel, string category, string message, Exception? exception)
    {
        var builder = new StringBuilder()
            .Append('[')
            .Append(_timeProvider.GetUtcNow().ToString("O", CultureInfo.InvariantCulture))
            .Append("] [")
            .Append(logLevel)
            .Append("] ")
            .Append(category)
            .Append(": ")
            .Append(message);
        if (exception is not null)
        {
            builder.Append(" | ").Append(exception);
        }

        builder.Append('\n');
        var line = Redact(builder.ToString());

        lock (_gate)
        {
            try
            {
                RollIfOversized(line);
                File.AppendAllText(_logFilePath, line);
            }
            catch (Exception writeFailure) when (writeFailure is IOException or UnauthorizedAccessException)
            {
                // Logging is best-effort: a full, locked, or unwritable disk must never surface into the
                // caller's control flow. An ILogger that threw here could break the hub reconnect loop or
                // the shutdown/cleanup path that logs while tearing down.
            }
        }
    }

    private void RollIfOversized(string pending)
    {
        var info = new FileInfo(_logFilePath);
        if (!info.Exists || info.Length + Encoding.UTF8.GetByteCount(pending) <= _maxBytes)
        {
            return;
        }

        File.Move(_logFilePath, _logFilePath + ".1", overwrite: true);
    }

    private sealed class FileLogger(TerminalFileLoggerProvider provider, string category) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);
            if (!IsEnabled(logLevel))
            {
                return;
            }

            provider.Write(logLevel, category, formatter(state, exception), exception);
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
