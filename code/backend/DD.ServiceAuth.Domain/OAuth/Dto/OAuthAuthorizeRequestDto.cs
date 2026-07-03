using System.Diagnostics.CodeAnalysis;

namespace DD.ServiceAuth.Domain.OAuth.Dto;

[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth redirect_uri is bound verbatim from the SPA and must be compared as an exact string, not URI-normalized.")]
[SuppressMessage(
    "Design",
    "CA1056:URI-like properties should not be strings",
    Justification = "OAuth redirect_uri is bound verbatim from the SPA and must be compared as an exact string, not URI-normalized.")]
public sealed record OAuthAuthorizeRequestDto(
    string? Action,
    string? ClientId,
    string? RedirectUri,
    string? CodeChallenge,
    string? State);
