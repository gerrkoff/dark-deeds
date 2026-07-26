namespace DD.TerminalClient.Domain.Abstractions;

// Abstracts reading one key press so the event loop can await keyboard input without depending on
// Spectre or System.Console directly. Returns the pressed key, or null when input ends (redirected input
// reaches EOF). Implementations must honour the cancellation token so a pending read unblocks promptly on
// shutdown; the Details adapter wraps IAnsiConsole.Input.ReadKeyAsync(intercept: true, ...) and tests
// supply a queued fake so a run never blocks on real input.
public interface IKeyInputSource
{
    Task<ConsoleKeyInfo?> ReadKeyAsync(CancellationToken cancellationToken);
}
