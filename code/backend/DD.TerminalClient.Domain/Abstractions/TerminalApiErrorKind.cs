namespace DD.TerminalClient.Domain.Abstractions;

// The four failure classes the terminal distinguishes when a backend call does not succeed. The sync
// layer reacts differently to each: Unauthorized returns to login, Transport schedules a retry while
// keeping the durable outbox, Validation surfaces a message, and Protocol marks an unexpected server
// contract violation. Deliberately coarse so callers never need to inspect raw HTTP status codes.
public enum TerminalApiErrorKind
{
    Unauthorized,
    Transport,
    Validation,
    Protocol,
}
