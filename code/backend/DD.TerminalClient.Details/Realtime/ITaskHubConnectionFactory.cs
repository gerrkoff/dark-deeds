namespace DD.TerminalClient.Details.Realtime;

// Creates the single hub connection the client owns. The token provider is captured, not the token, so
// the connection reads a fresh JWT every time it negotiates - a renewal or logout takes effect on the
// next reconnect without rebuilding anything. Isolating creation behind this factory lets tests supply a
// fake connection while production builds a real SignalR one.
internal interface ITaskHubConnectionFactory
{
    ITaskHubConnection Create(Func<string?> tokenProvider);
}
