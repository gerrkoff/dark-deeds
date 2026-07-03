using System.Diagnostics.CodeAnalysis;

namespace DD.ServiceAuth.Domain.OAuth.Services;

public interface IOAuthUrlService
{
    bool IsAllowedRedirectUri([NotNullWhen(true)] string? redirectUri);

    string BuildSuccessRedirect(string redirectUri, string code, string state);

    string BuildErrorRedirect(string redirectUri, string errorCode, string state);
}

internal sealed class OAuthUrlService : IOAuthUrlService
{
    // VS Code registers well-known Microsoft redirect helpers (stable + Insiders) alongside its
    // loopback addresses, and dynamic client registration sends the whole redirect_uris set at
    // once, so allow these exact URIs; otherwise registration and the browser consent flow fail.
    // PKCE still protects the code exchange for these non-loopback redirects.
    private static readonly string[] AllowedExternalRedirectUris =
    [
        "https://vscode.dev/redirect",
        "https://insiders.vscode.dev/redirect",
    ];

    public bool IsAllowedRedirectUri([NotNullWhen(true)] string? redirectUri)
    {
        if (string.IsNullOrEmpty(redirectUri) ||
            !Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (AllowedExternalRedirectUris.Contains(redirectUri, StringComparer.Ordinal))
        {
            return true;
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.Ordinal) &&
            !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.Ordinal))
        {
            return false;
        }

        return string.IsNullOrEmpty(uri.Fragment) && uri.IsLoopback;
    }

    public string BuildSuccessRedirect(string redirectUri, string code, string state)
    {
        return Append(redirectUri, ("code", code), ("state", state));
    }

    public string BuildErrorRedirect(string redirectUri, string errorCode, string state)
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
