namespace DD.TerminalClient.Domain.Abstractions;

// The single failure type raised by the auth and task API clients, carrying a coarse classification
// (Kind) so callers switch on intent rather than raw HTTP status codes or exception subtypes. Never
// carries request bodies or tokens in its message, so a logged or surfaced error can never leak a
// credential. The standard exception constructors default to the Protocol (unexpected) class.
public sealed class TerminalApiException : Exception
{
    public TerminalApiException()
        : this(TerminalApiErrorKind.Protocol, "An unexpected terminal API error occurred.")
    {
    }

    public TerminalApiException(string message)
        : this(TerminalApiErrorKind.Protocol, message)
    {
    }

    public TerminalApiException(string message, Exception innerException)
        : this(TerminalApiErrorKind.Protocol, message, innerException)
    {
    }

    public TerminalApiException(TerminalApiErrorKind kind, string message)
        : base(message)
    {
        Kind = kind;
    }

    public TerminalApiException(TerminalApiErrorKind kind, string message, Exception innerException)
        : base(message, innerException)
    {
        Kind = kind;
    }

    public TerminalApiErrorKind Kind { get; }
}
