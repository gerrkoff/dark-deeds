namespace DD.TerminalClient.SelfTest;

// A stage-labelled, secret-safe self-test failure. The message is always safe to print because callers
// build it only from fixed text and coarse enumeration values, never from a request body, token, or raw
// exception text. Stage names the contract step that failed so a diagnostic pinpoints it.
public sealed class SelfTestException : Exception
{
    public SelfTestException()
    {
    }

    public SelfTestException(string message)
        : base(message)
    {
    }

    public SelfTestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public SelfTestException(string stage, string message, Exception? innerException)
        : base(message, innerException)
    {
        Stage = stage;
    }

    public string Stage { get; } = string.Empty;
}
