using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Application;
using DD.TerminalClient.Domain.Realtime;
using DD.TerminalClient.Domain.Time;

namespace DD.TerminalClient;

// Everything the event loop is composed from, gathered into one value so the composition root and the
// tests build the application the same way. The adapters are the Domain abstractions (so tests supply
// fakes), plus the pure reducer, the clock, the token setter shared with the HTTP/hub layers, a terminal
// dimension reader, and injectable delays/timer intervals so tests stay deterministic and never wait on
// wall-clock time. The hub is built through a factory the application calls with its own event sink, so
// the loop remains the sole writer of state.
internal sealed record TerminalDependencies
{
    public required IAuthApiClient Auth { get; init; }

    public required ITaskApiClient Tasks { get; init; }

    public required Func<Action<TaskHubEvent>, ITaskHubClient> HubFactory { get; init; }

    public required ILocalStateStore StateStore { get; init; }

    public required ITokenStore TokenStore { get; init; }

    public required IKeyInputSource Keys { get; init; }

    public required ITerminalRenderer Renderer { get; init; }

    public required ApplicationReducer Reducer { get; init; }

    public required ILocalDateProvider LocalDate { get; init; }

    public required TimeProvider Clock { get; init; }

    // Pushes the current token to the HTTP handler and hub token provider on sign-in, renewal and 401.
    public required Action<string?> SetToken { get; init; }

    public required Func<(int Width, int Height)> ReadDimensions { get; init; }

    public required string ProfileName { get; init; }

    // Injectable so retry and timer waits are immediate under test; production uses Task.Delay.
    public Func<TimeSpan, CancellationToken, Task> Delay { get; init; } = Task.Delay;

    public TimeSpan RenewInterval { get; init; } = TimeSpan.FromMinutes(15);

    public TimeSpan ResizePollInterval { get; init; } = TimeSpan.FromMilliseconds(500);

    // Tests leave the wall-clock timers off and enqueue tick events directly for determinism.
    public bool StartBackgroundTimers { get; init; } = true;
}
