using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Realtime;

namespace DD.TerminalClient.SelfTest;

// Everything the self-test is composed from, gathered into one value so the composition root and the
// tests build it the same way. Auth and the writer task client are the real adapters (fakes under test);
// the two hub factories build connections with distinct client ids so the observer receives the writer's
// own saves; SetToken shares the JWT with the HTTP and hub layers after sign-in; Report is the
// secret-safe diagnostics sink; and the uid/title/timeout hooks keep every run unique and every wait
// finite.
internal sealed record SelfTestContext
{
    public required IAuthApiClient Auth { get; init; }

    public required ITaskApiClient Writer { get; init; }

    public required Func<Action<TaskHubEvent>, ITaskHubClient> WriterHubFactory { get; init; }

    public required Func<Action<TaskHubEvent>, ITaskHubClient> ObserverHubFactory { get; init; }

    public required Action<string?> SetToken { get; init; }

    public required Action<string> Report { get; init; }

    public required SelfTestCredentials Credentials { get; init; }

    public Func<string> NewUid { get; init; } = () => Guid.NewGuid().ToString();

    public Func<string> NewTitle { get; init; } = () => $"dd-terminal self-test {Guid.NewGuid():N}";

    public TimeSpan EventTimeout { get; init; } = TimeSpan.FromSeconds(30);
}
