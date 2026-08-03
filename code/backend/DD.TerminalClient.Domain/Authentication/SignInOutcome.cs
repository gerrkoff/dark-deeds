namespace DD.TerminalClient.Domain.Authentication;

// The outcome of a sign-in call: a status plus the established session when, and only when, the status
// is Success. A non-success outcome carries no session.
public sealed record SignInOutcome(TerminalSignInStatus Status, AuthSession? Session);
