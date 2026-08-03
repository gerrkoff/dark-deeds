namespace DD.TerminalClient.Domain.Authentication;

// The application-level result of a sign-in attempt. Distinct from a transport failure: wrong
// credentials is a normal, non-exceptional outcome (the server answers 200 with a result code), so the
// login flow can re-prompt rather than treat it as an error.
public enum TerminalSignInStatus
{
    Success,
    InvalidCredentials,
    Failed,
}
