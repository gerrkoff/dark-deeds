using DD.TerminalClient.Domain.Abstractions;
using Spectre.Console;

namespace DD.TerminalClient.Details.Ui;

// The real keyboard behind IKeyInputSource: it reads one intercepted key from the Spectre console so the
// key is consumed rather than echoed into the alternate screen, and forwards the cancellation token so a
// read in progress unblocks when the event loop shuts down. It holds no state and never renders; the
// event loop awaits it and hands each key to TerminalInputReducer.
public sealed class TerminalInputReader(IAnsiConsole console) : IKeyInputSource
{
    private readonly IAnsiConsole console = console ?? throw new ArgumentNullException(nameof(console));

    public Task<ConsoleKeyInfo?> ReadKeyAsync(CancellationToken cancellationToken)
    {
        return console.Input.ReadKeyAsync(intercept: true, cancellationToken);
    }
}
