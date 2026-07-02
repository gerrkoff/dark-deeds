namespace DD.ServiceAuth.Domain.OAuth;

public static class OAuthConstants
{
    public const string GrantAuthorizationCode = "authorization_code";

    public const string GrantRefreshToken = "refresh_token";

    public const string ResponseTypeCode = "code";

    public const string CodeChallengeMethodS256 = "S256";

    public const string ActionAllow = "allow";

    public const string AccessDeniedError = "access_denied";

    // Audience that scopes OAuth-issued access tokens to the MCP resource (the /mcp endpoint).
    // Public because the JwtBearer setup in DD.ServiceAuth.Details must reference it.
    public const string AccessTokenAudience = "dd-oauth-access";
}
