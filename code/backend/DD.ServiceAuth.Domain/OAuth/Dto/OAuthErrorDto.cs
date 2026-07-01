using System.Text.Json.Serialization;

namespace DD.ServiceAuth.Domain.OAuth.Dto;

public sealed record OAuthErrorDto(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("error_description")] string ErrorDescription)
{
    public static OAuthErrorDto RedirectUriNotLoopback { get; } =
        new("invalid_request", "redirect_uri must be a loopback address.");

    public static OAuthErrorDto ClientIdRequired { get; } =
        new("invalid_request", "client_id is required.");

    public static OAuthErrorDto StateRequired { get; } =
        new("invalid_request", "state is required.");

    public static OAuthErrorDto PkceChallengeRequired { get; } =
        new("invalid_request", "A PKCE code_challenge with code_challenge_method=S256 is required.");

    public static OAuthErrorDto CodeChallengeAndStateRequired { get; } =
        new("invalid_request", "code_challenge and state are required.");

    public static OAuthErrorDto UsernameRequired { get; } =
        new("invalid_request", "username is required.");

    public static OAuthErrorDto PasswordRequired { get; } =
        new("invalid_request", "password is required.");

    public static OAuthErrorDto AuthorizationCodeFieldsRequired { get; } =
        new("invalid_request", "code, redirect_uri, client_id and code_verifier are required.");

    public static OAuthErrorDto RefreshTokenRequired { get; } =
        new("invalid_request", "refresh_token is required.");

    public static OAuthErrorDto UnsupportedResponseType { get; } =
        new("unsupported_response_type", "response_type must be 'code'.");

    public static OAuthErrorDto UnsupportedGrantType { get; } =
        new("unsupported_grant_type", "grant_type must be 'authorization_code' or 'refresh_token'.");

    public static OAuthErrorDto RegistrationRedirectUrisNotLoopback { get; } =
        new("invalid_redirect_uri", "All redirect_uris must be loopback addresses.");

    public static OAuthErrorDto InvalidAuthorizationCode { get; } =
        new("invalid_grant", "The authorization code or PKCE verifier is invalid.");

    public static OAuthErrorDto InvalidRefreshToken { get; } =
        new("invalid_grant", "The refresh token is invalid or expired.");

    public static OAuthErrorDto UserNotFound { get; } =
        new("invalid_grant", "The user account no longer exists.");
}
