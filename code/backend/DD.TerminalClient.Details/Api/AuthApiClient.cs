using System.Net.Http.Json;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Authentication;

namespace DD.TerminalClient.Details.Api;

// Talks to the existing anonymous sign-in and bearer-authenticated renew endpoints. Sign-in maps the
// server's numeric result code to an application outcome (success, wrong credentials, or other failure)
// without throwing for a rejected password; renew returns the fresh JWT wrapped as a session. Request
// and response wire shapes are private to this client - only the shared task contract is public.
internal sealed class AuthApiClient(HttpClient httpClient)
    : TerminalHttpClientBase(httpClient), IAuthApiClient
{
    private const string SignInPath = "api/auth/account/signin";
    private const string RenewPath = "api/auth/account/renew";

    private enum SignInResultCode
    {
        Unknown = 0,
        Success = 1,
        WrongUsernamePassword = 2,
    }

    public async Task<SignInOutcome> SignInAsync(
        string username, string password, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(password);

        using var request = new HttpRequestMessage(HttpMethod.Post, SignInPath)
        {
            Content = JsonContent.Create(new SignInRequest(username, password), options: JsonOptions),
        };

        using var response = await SendAsync(request, cancellationToken);
        EnsureSuccess(response);
        var body = await ReadJsonAsync<SignInResponse>(response, cancellationToken);

        if (body.Result == SignInResultCode.Success && !string.IsNullOrWhiteSpace(body.Token))
        {
            return new SignInOutcome(TerminalSignInStatus.Success, AuthSession.FromToken(body.Token));
        }

        if (body.Result == SignInResultCode.WrongUsernamePassword)
        {
            return new SignInOutcome(TerminalSignInStatus.InvalidCredentials, null);
        }

        return new SignInOutcome(TerminalSignInStatus.Failed, null);
    }

    public async Task<AuthSession> RenewTokenAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, RenewPath);

        using var response = await SendAsync(request, cancellationToken);
        EnsureSuccess(response);
        var token = (await ReadTextAsync(response, cancellationToken)).Trim();

        return token.Length == 0
            ? throw new TerminalApiException(
                TerminalApiErrorKind.Protocol, "The renew response contained no token.")
            : AuthSession.FromToken(token);
    }

    private sealed record SignInRequest(string Username, string Password);

    private sealed record SignInResponse
    {
        public string Token { get; init; } = string.Empty;

        public SignInResultCode Result { get; init; }
    }
}
