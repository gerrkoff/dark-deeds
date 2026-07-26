namespace DD.TerminalClient.Domain.State;

// Raised when persisted state cannot be loaded because it is malformed or was written by a newer,
// unsupported schema. The offending file is retained untouched; the caller must surface this as a
// blocking, actionable error rather than silently discarding the user's cache and outbox.
public sealed class TerminalStateException : Exception
{
    public TerminalStateException()
    {
    }

    public TerminalStateException(string message)
        : base(message)
    {
    }

    public TerminalStateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
