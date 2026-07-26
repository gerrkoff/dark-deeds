namespace DD.TerminalClient.Domain.Input;

// The outcome of reducing one key: the next input state, the application command to run (None when the
// key only changed the mode or edited the buffer), the committed text a Submit* command acts on, and an
// optional status message for a command declined because of missing focus or date context.
public sealed record TerminalInputResult
{
    public required TerminalInputState State { get; init; }

    public TerminalCommand Command { get; init; } = TerminalCommand.None;

    public string? CommittedText { get; init; }

    public string? StatusMessage { get; init; }

    public static TerminalInputResult Unchanged(TerminalInputState state)
    {
        return new TerminalInputResult { State = state };
    }
}
