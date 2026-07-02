namespace DD.ServiceAuth.Domain.OAuth;

// Internal JWT issuer/audience markers that discriminate the OAuth token kinds (authorization
// code vs refresh token vs access token). They are never seen by clients and must stay in sync
// between the issuing and verifying sides, so they are hardcoded invariants rather than settings:
// distinct audiences are what prevent one grant's token from being replayed as another.
internal static class OAuthTokenKinds
{
    public const string Issuer = "dd-oauth";

    public const string AuthorizationCodeAudience = "dd-oauth-authcode";

    public const string RefreshTokenAudience = "dd-oauth-refresh";
}
