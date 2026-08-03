using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DD.TerminalClient.Domain.Abstractions;

namespace DD.TerminalClient.Details.Api;

// Shared plumbing for the REST clients: one web-defaults JSON contract (camelCase names, numeric enums)
// matching the server, plus send/read helpers that translate low-level failures into the four
// TerminalApiException classes without any broad catch. A caller-requested cancellation is always
// allowed to propagate; only a timeout (the token was not the caller's) is reclassified as transport.
internal abstract class TerminalHttpClientBase(HttpClient httpClient)
{
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected HttpClient HttpClient { get; } =
        httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    protected async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await HttpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new TerminalApiException(
                TerminalApiErrorKind.Transport, "The request could not reach the server.", exception);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TerminalApiException(TerminalApiErrorKind.Transport, "The request timed out.");
        }
    }

    protected static void EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = $"The server responded with status code {(int)response.StatusCode}.";
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new TerminalApiException(TerminalApiErrorKind.Unauthorized, message);
        }

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.UnprocessableEntity)
        {
            throw new TerminalApiException(TerminalApiErrorKind.Validation, message);
        }

        throw new TerminalApiException(TerminalApiErrorKind.Protocol, message);
    }

    protected static async Task<T> ReadJsonAsync<T>(
        HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentType?.MediaType is not "application/json")
        {
            throw new TerminalApiException(
                TerminalApiErrorKind.Protocol, "The server response was not JSON.");
        }

        try
        {
            var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
            return value ?? throw new TerminalApiException(
                TerminalApiErrorKind.Protocol, "The server returned an empty response body.");
        }
        catch (JsonException exception)
        {
            throw new TerminalApiException(
                TerminalApiErrorKind.Protocol, "The server response could not be parsed.", exception);
        }
        catch (HttpRequestException exception)
        {
            throw new TerminalApiException(
                TerminalApiErrorKind.Transport, "The response could not be read from the server.", exception);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TerminalApiException(
                TerminalApiErrorKind.Transport, "Reading the response timed out.");
        }
    }

    protected static async Task<string> ReadTextAsync(
        HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new TerminalApiException(
                TerminalApiErrorKind.Transport, "The response could not be read from the server.", exception);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TerminalApiException(
                TerminalApiErrorKind.Transport, "Reading the response timed out.");
        }
    }
}
