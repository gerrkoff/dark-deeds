namespace DD.ServiceAuth.Domain.OAuth;

public static class OAuthConstants
{
    public const string GrantAuthorizationCode = "authorization_code";

    public const string GrantRefreshToken = "refresh_token";

    public const string ResponseTypeCode = "code";

    public const string CodeChallengeMethodS256 = "S256";

    public const string ActionAllow = "allow";

    public const string AccessDeniedError = "access_denied";

    public const string VsCodeRedirectUri = "https://vscode.dev/redirect";
}
