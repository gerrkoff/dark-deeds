using DD.TerminalClient.Domain.Authentication;

namespace DD.TerminalClient.Domain.Abstractions;

// Authentication calls against the existing server contract. Sign-in is anonymous and returns an
// application outcome (including wrong credentials) rather than throwing; renewal is bearer-authenticated
// and returns a fresh session. Both surface transport, validation, and protocol problems as
// TerminalApiException. Implementations parse the JWT only for local UX, never as server validation.
public interface IAuthApiClient
{
    Task<SignInOutcome> SignInAsync(string username, string password, CancellationToken cancellationToken);

    Task<AuthSession> RenewTokenAsync(CancellationToken cancellationToken);
}
