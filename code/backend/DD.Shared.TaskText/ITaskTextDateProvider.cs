namespace DD.Shared.TaskText;

// Supplies the current local calendar date used to resolve MMDD and relative (!) dates.
// Injected so parsing stays deterministic and testable without any wall-clock dependency.
public interface ITaskTextDateProvider
{
    DateOnly Today { get; }
}
