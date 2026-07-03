namespace DD.ServiceAuth.Domain.OAuth.Dto;

public sealed record OAuthAuthorizeRequestDto(
    string? Action,
    string? ClientId,
    string? RedirectUri,
    string? CodeChallenge,
    string? State);
