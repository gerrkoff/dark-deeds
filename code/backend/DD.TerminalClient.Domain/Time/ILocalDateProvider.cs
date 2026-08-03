namespace DD.TerminalClient.Domain.Time;

// Supplies the current local calendar date to the Domain without any wall-clock dependency, so
// date-relative behavior (year elision in the editor string, focus-date defaults, one-day moves)
// stays deterministic and testable. The Details layer provides the real system-clock implementation.
public interface ILocalDateProvider
{
    DateOnly Today { get; }
}
