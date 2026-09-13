using DD.ServiceAuth.Domain.Dto;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class AuthReader
{
    internal static Task<CurrentUserDto> ReadCurrentUserAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<CurrentUserDto>(
            response,
            "current user",
            cancellationToken);
    }

    internal static Task<SignUpResultDto> ReadSignUpAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<SignUpResultDto>(
            response,
            "sign-up",
            cancellationToken);
    }

    internal static Task<SignInResultDto> ReadSignInAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<SignInResultDto>(
            response,
            "sign-in",
            cancellationToken);
    }

    internal static Task<string> ReadTokenAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadStringAsync(response, cancellationToken);
    }
}
