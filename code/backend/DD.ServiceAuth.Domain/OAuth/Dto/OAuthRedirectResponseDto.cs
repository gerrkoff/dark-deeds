using System.Diagnostics.CodeAnalysis;

namespace DD.ServiceAuth.Domain.OAuth.Dto;

[SuppressMessage(
    "Design",
    "CA1054:URI-like parameters should not be strings",
    Justification = "OAuth redirect URL is emitted verbatim to the SPA and must not be URI-normalized.")]
[SuppressMessage(
    "Design",
    "CA1056:URI-like properties should not be strings",
    Justification = "OAuth redirect URL is emitted verbatim to the SPA and must not be URI-normalized.")]
public sealed record OAuthRedirectResponseDto(string RedirectUrl);
