using System.Net.Http.Json;
using DD.ServiceAuth.Domain.Dto;

namespace DD.Tests.Integration.Infrastructure.Api;

internal static class AuthApi
{
    private static readonly Uri AccountUri = new("api/auth/account", UriKind.Relative);
    private static readonly Uri RenewUri = new("api/auth/account/renew", UriKind.Relative);
    private static readonly Uri SignInUri = new("api/auth/account/signin", UriKind.Relative);
    private static readonly Uri SignUpUri = new("api/auth/account/signup", UriKind.Relative);

    internal static Task<HttpResponseMessage> GetCurrentUserAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(AccountUri, cancellationToken);
    }

    internal static Task<HttpResponseMessage> SignUpAsync(
        HttpClient client,
        SignUpInfoDto request,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(SignUpUri, request, cancellationToken);
    }

    internal static Task<HttpResponseMessage> SignInAsync(
        HttpClient client,
        SignInInfoDto request,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(SignInUri, request, cancellationToken);
    }

    internal static Task<HttpResponseMessage> RenewAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsync(RenewUri, content: null, cancellationToken);
    }
}
