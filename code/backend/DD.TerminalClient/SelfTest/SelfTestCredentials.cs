namespace DD.TerminalClient.SelfTest;

// The credentials the self-test signs in with, read from the environment. Kept as a small validated
// value so the missing-variable check and the run share one shape.
internal sealed record SelfTestCredentials(string Username, string Password);
