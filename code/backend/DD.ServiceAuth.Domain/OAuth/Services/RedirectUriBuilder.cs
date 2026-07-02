using System.Diagnostics.CodeAnalysis;

namespace DD.ServiceAuth.Domain.OAuth.Services;

[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth redirect_uri must be preserved and appended to as an exact string, not URI-normalized.")]
internal interface IRedirectUriBuilder
{
    string BuildSuccess(string redirectUri, string code, string state);

    string BuildError(string redirectUri, string errorCode, string state);
}

[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth redirect_uri must be preserved and appended to as an exact string, not URI-normalized.")]
internal sealed class RedirectUriBuilder : IRedirectUriBuilder
{
    public string BuildSuccess(string redirectUri, string code, string state)
    {
        return Append(redirectUri, ("code", code), ("state", state));
    }

    public string BuildError(string redirectUri, string errorCode, string state)
    {
        return Append(redirectUri, ("error", errorCode), ("state", state));
    }

    private static string Append(string redirectUri, params (string Key, string Value)[] parameters)
    {
        var separator = redirectUri.Contains('?', StringComparison.Ordinal) ? '&' : '?';
        var query = string.Join(
            '&',
            parameters.Select(parameter => $"{parameter.Key}={Uri.EscapeDataString(parameter.Value)}"));

        return $"{redirectUri}{separator}{query}";
    }
}
