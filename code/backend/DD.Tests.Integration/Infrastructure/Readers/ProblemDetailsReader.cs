using Microsoft.AspNetCore.Mvc;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class ProblemDetailsReader
{
    internal static Task<ProblemDetails> ReadAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<ProblemDetails>(
            response,
            ensureSuccess: false,
            cancellationToken);
    }
}
