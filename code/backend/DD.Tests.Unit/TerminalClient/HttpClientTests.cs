using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DD.TerminalClient.Details.Api;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Authentication;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Time;
using Moq;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Fake-handler coverage of the REST layer against the exact server contract: sign-in/renew/load/save
// routes, query, casing, media types, task JSON fields, identity headers, save-response application,
// date mapping by UTC components, finite timeouts, caller cancellation, and every TerminalApiException
// class. No real socket is opened - a StubHttpMessageHandler records requests and scripts responses.
public sealed class HttpClientTests
{
    private const string BaseAddress = "https://example.test/";
    private const string ClientId = "11111111-1111-1111-1111-111111111111";

    [Fact]
    public async Task SignInAsync_PostsToSignInRoute_WithCamelCaseJsonBody()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, "{\"token\":\"\",\"result\":2}")));
        var client = new AuthApiClient(CreateHttpClient(stub));

        await client.SignInAsync("alice", "s3cret", CancellationToken.None);

        Assert.Equal(HttpMethod.Post, stub.LastRequest!.Method);
        Assert.Equal("/api/auth/account/signin", stub.LastRequest.RequestUri!.AbsolutePath);
        Assert.Equal("application/json", stub.LastRequestContentType!.MediaType);

        using var body = JsonDocument.Parse(stub.LastRequestBody!);
        Assert.Equal("alice", body.RootElement.GetProperty("username").GetString());
        Assert.Equal("s3cret", body.RootElement.GetProperty("password").GetString());
    }

    [Fact]
    public async Task SignInAsync_Success_ReturnsSessionWithParsedUsernameAndExpiry()
    {
        var expiry = DateTimeOffset.FromUnixTimeSeconds(2000000000);
        var token = CreateJwt("alice", expiry);
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(
            JsonResponse(HttpStatusCode.OK, $"{{\"token\":\"{token}\",\"result\":1}}")));
        var client = new AuthApiClient(CreateHttpClient(stub));

        var outcome = await client.SignInAsync("alice", "s3cret", CancellationToken.None);

        Assert.Equal(TerminalSignInStatus.Success, outcome.Status);
        Assert.NotNull(outcome.Session);
        Assert.Equal(token, outcome.Session!.Token);
        Assert.Equal("alice", outcome.Session.Username);
        Assert.Equal(expiry, outcome.Session.ExpiresAt);
    }

    [Fact]
    public async Task SignInAsync_WrongCredentials_ReturnsInvalidCredentialsWithoutSession()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, "{\"token\":\"\",\"result\":2}")));
        var client = new AuthApiClient(CreateHttpClient(stub));

        var outcome = await client.SignInAsync("alice", "wrong", CancellationToken.None);

        Assert.Equal(TerminalSignInStatus.InvalidCredentials, outcome.Status);
        Assert.Null(outcome.Session);
    }

    [Fact]
    public async Task SignInAsync_UnknownResult_ReturnsFailed()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, "{\"token\":\"\",\"result\":0}")));
        var client = new AuthApiClient(CreateHttpClient(stub));

        var outcome = await client.SignInAsync("alice", "s3cret", CancellationToken.None);

        Assert.Equal(TerminalSignInStatus.Failed, outcome.Status);
        Assert.Null(outcome.Session);
    }

    [Fact]
    public async Task SignInAsync_SuccessResultButEmptyToken_ReturnsFailedWithoutSession()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, "{\"token\":\"   \",\"result\":1}")));
        var client = new AuthApiClient(CreateHttpClient(stub));

        var outcome = await client.SignInAsync("alice", "s3cret", CancellationToken.None);

        Assert.Equal(TerminalSignInStatus.Failed, outcome.Status);
        Assert.Null(outcome.Session);
    }

    [Fact]
    public async Task RenewTokenAsync_PostsToRenewRoute_AndReadsPlainTextToken()
    {
        var expiry = DateTimeOffset.FromUnixTimeSeconds(1900000000);
        var token = CreateJwt("bob", expiry);
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(TextResponse(HttpStatusCode.OK, token)));
        var client = new AuthApiClient(CreateHttpClient(stub));

        var session = await client.RenewTokenAsync(CancellationToken.None);

        Assert.Equal(HttpMethod.Post, stub.LastRequest!.Method);
        Assert.Equal("/api/auth/account/renew", stub.LastRequest.RequestUri!.AbsolutePath);
        Assert.Equal(token, session.Token);
        Assert.Equal("bob", session.Username);
        Assert.Equal(expiry, session.ExpiresAt);
    }

    [Fact]
    public async Task RenewTokenAsync_EmptyBody_ThrowsProtocol()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(TextResponse(HttpStatusCode.OK, "   ")));
        var client = new AuthApiClient(CreateHttpClient(stub));

        var exception = await Assert.ThrowsAsync<TerminalApiException>(
            () => client.RenewTokenAsync(CancellationToken.None));
        Assert.Equal(TerminalApiErrorKind.Protocol, exception.Kind);
    }

    [Fact]
    public async Task RenewTokenAsync_Unauthorized_ThrowsUnauthorized()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)));
        var client = new AuthApiClient(CreateHttpClient(stub));

        var exception = await Assert.ThrowsAsync<TerminalApiException>(
            () => client.RenewTokenAsync(CancellationToken.None));
        Assert.Equal(TerminalApiErrorKind.Unauthorized, exception.Kind);
    }

    [Fact]
    public async Task TerminalHttpHandler_AddsBearerAuthorizationAndClientIdHeaders()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(TextResponse(HttpStatusCode.OK, CreateJwt("bob", DateTimeOffset.UnixEpoch))));
        var client = CreateHttpClient(stub, () => "the-access-token");
        var authClient = new AuthApiClient(client);

        await authClient.RenewTokenAsync(CancellationToken.None);

        Assert.Equal("Bearer", stub.LastRequest!.Headers.Authorization!.Scheme);
        Assert.Equal("the-access-token", stub.LastRequest.Headers.Authorization.Parameter);
        Assert.Equal(ClientId, Assert.Single(stub.LastRequest.Headers.GetValues(TerminalHttpHandler.ClientIdHeader)));
    }

    [Fact]
    public async Task TerminalHttpHandler_WithoutToken_OmitsAuthorizationButKeepsClientId()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, "{\"token\":\"\",\"result\":2}")));
        var client = CreateHttpClient(stub, () => null);
        var authClient = new AuthApiClient(client);

        await authClient.SignInAsync("alice", "s3cret", CancellationToken.None);

        Assert.Null(stub.LastRequest!.Headers.Authorization);
        Assert.Equal(ClientId, Assert.Single(stub.LastRequest.Headers.GetValues(TerminalHttpHandler.ClientIdHeader)));
    }

    [Fact]
    public async Task LoadTasksAsync_GetsTasksRoute_FromCurrentLocalMondayMappedToUtcMidnight()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.OK, "[]")));

        // 2026-07-23 is a Thursday; the visible period starts on Monday 2026-07-20.
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));

        await client.LoadTasksAsync(CancellationToken.None);

        Assert.Equal(HttpMethod.Get, stub.LastRequest!.Method);
        Assert.Equal("/api/task/tasks", stub.LastRequest.RequestUri!.AbsolutePath);
        Assert.Equal("2026-07-20T00:00:00.000Z", QueryParam(stub.LastRequest.RequestUri, "from"));
    }

    [Fact]
    public async Task LoadTasksAsync_MapsEveryTaskField()
    {
        const string json =
            "[{\"uid\":\"u1\",\"title\":\"Meeting\",\"date\":\"2026-07-26T00:00:00Z\",\"time\":1050," +
            "\"order\":3,\"completed\":true,\"deleted\":false,\"type\":2,\"isProbable\":true,\"version\":5}]";
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(HttpStatusCode.OK, json)));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));

        var tasks = await client.LoadTasksAsync(CancellationToken.None);

        var task = Assert.Single(tasks);
        Assert.Equal("u1", task.Uid);
        Assert.Equal("Meeting", task.Title);
        Assert.Equal(new DateOnly(2026, 7, 26), task.Date);
        Assert.Equal(1050, task.Time);
        Assert.Equal(3, task.Order);
        Assert.True(task.Completed);
        Assert.False(task.Deleted);
        Assert.Equal(TerminalTaskType.Routine, task.Type);
        Assert.True(task.IsProbable);
        Assert.Equal(5, task.Version);
    }

    [Fact]
    public async Task LoadTasksAsync_NullDate_MapsToNoDate()
    {
        const string json =
            "[{\"uid\":\"u1\",\"title\":\"No date\",\"date\":null,\"time\":null,\"order\":0," +
            "\"completed\":false,\"deleted\":false,\"type\":0,\"isProbable\":false,\"version\":1}]";
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(HttpStatusCode.OK, json)));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));

        var task = Assert.Single(await client.LoadTasksAsync(CancellationToken.None));

        Assert.Null(task.Date);
        Assert.Null(task.Time);
    }

    [Fact]
    public async Task SaveTasksAsync_PostsTaskArray_WithAllJsonFields()
    {
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(HttpStatusCode.OK, "[]")));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));
        var task = new TerminalTask
        {
            Uid = "u1",
            Title = "Meeting",
            Date = new DateOnly(2026, 7, 26),
            Time = 1050,
            Order = 3,
            Completed = true,
            Deleted = false,
            Type = TerminalTaskType.Weekly,
            IsProbable = true,
            Version = 7,
        };

        await client.SaveTasksAsync([task], CancellationToken.None);

        Assert.Equal(HttpMethod.Post, stub.LastRequest!.Method);
        Assert.Equal("/api/task/tasks", stub.LastRequest.RequestUri!.AbsolutePath);
        Assert.Equal("application/json", stub.LastRequestContentType!.MediaType);

        using var document = JsonDocument.Parse(stub.LastRequestBody!);
        var element = Assert.Single(EnumerateArray(document.RootElement));
        Assert.Equal("u1", element.GetProperty("uid").GetString());
        Assert.Equal("Meeting", element.GetProperty("title").GetString());
        Assert.Equal(1050, element.GetProperty("time").GetInt32());
        Assert.Equal(3, element.GetProperty("order").GetInt32());
        Assert.True(element.GetProperty("completed").GetBoolean());
        Assert.False(element.GetProperty("deleted").GetBoolean());
        Assert.True(element.GetProperty("isProbable").GetBoolean());
        Assert.Equal(7, element.GetProperty("version").GetInt32());

        // Type is serialized numerically (Weekly = 3), matching the server's default enum handling.
        Assert.Equal(3, element.GetProperty("type").GetInt32());

        // The local date crosses the wire as that same calendar day at UTC midnight - no timezone shift.
        Assert.Equal(
            new DateTimeOffset(2026, 7, 26, 0, 0, 0, TimeSpan.Zero),
            DateTimeOffset.Parse(
                element.GetProperty("date").GetString()!,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind));
    }

    [Fact]
    public async Task SaveTasksAsync_NoDateTask_SerializesNullDate()
    {
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(HttpStatusCode.OK, "[]")));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));
        var task = new TerminalTask { Uid = "u1", Title = "No date", Date = null, Time = null };

        await client.SaveTasksAsync([task], CancellationToken.None);

        using var document = JsonDocument.Parse(stub.LastRequestBody!);
        var element = Assert.Single(EnumerateArray(document.RootElement));
        Assert.Equal(JsonValueKind.Null, element.GetProperty("date").ValueKind);
        Assert.Equal(JsonValueKind.Null, element.GetProperty("time").ValueKind);
    }

    [Fact]
    public async Task SaveTasksAsync_AppliesSaveResponse_ReturningServerVersions()
    {
        const string response =
            "[{\"uid\":\"u1\",\"title\":\"Meeting\",\"date\":\"2026-07-26T00:00:00Z\",\"time\":1050," +
            "\"order\":3,\"completed\":false,\"deleted\":false,\"type\":0,\"isProbable\":false,\"version\":8}]";
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(HttpStatusCode.OK, response)));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));
        var task = new TerminalTask
        {
            Uid = "u1",
            Title = "Meeting",
            Date = new DateOnly(2026, 7, 26),
            Time = 1050,
            Order = 3,
            Version = 7,
        };

        var saved = Assert.Single(await client.SaveTasksAsync([task], CancellationToken.None));

        Assert.Equal("u1", saved.Uid);
        Assert.Equal(8, saved.Version);
        Assert.Equal(new DateOnly(2026, 7, 26), saved.Date);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, TerminalApiErrorKind.Unauthorized)]
    [InlineData(HttpStatusCode.BadRequest, TerminalApiErrorKind.Validation)]
    [InlineData(HttpStatusCode.UnprocessableEntity, TerminalApiErrorKind.Validation)]
    [InlineData(HttpStatusCode.InternalServerError, TerminalApiErrorKind.Protocol)]
    [InlineData(HttpStatusCode.NotFound, TerminalApiErrorKind.Protocol)]
    public async Task LoadTasksAsync_ErrorStatus_ClassifiesException(
        HttpStatusCode status, TerminalApiErrorKind expected)
    {
        var stub = new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(status)));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));

        var exception = await Assert.ThrowsAsync<TerminalApiException>(
            () => client.LoadTasksAsync(CancellationToken.None));
        Assert.Equal(expected, exception.Kind);
    }

    [Fact]
    public async Task LoadTasksAsync_NonJsonSuccess_ThrowsProtocol()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            Task.FromResult(TextResponse(HttpStatusCode.OK, "not json")));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));

        var exception = await Assert.ThrowsAsync<TerminalApiException>(
            () => client.LoadTasksAsync(CancellationToken.None));
        Assert.Equal(TerminalApiErrorKind.Protocol, exception.Kind);
    }

    [Fact]
    public async Task LoadTasksAsync_NetworkFailure_ThrowsTransport()
    {
        var stub = new StubHttpMessageHandler((_, _) =>
            throw new HttpRequestException("connection refused"));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));

        var exception = await Assert.ThrowsAsync<TerminalApiException>(
            () => client.LoadTasksAsync(CancellationToken.None));
        Assert.Equal(TerminalApiErrorKind.Transport, exception.Kind);
    }

    [Fact]
    public async Task LoadTasksAsync_Timeout_ThrowsTransport()
    {
        // A timeout surfaces as TaskCanceledException whose token is not the caller's - it must be
        // reclassified as a transport failure, never leaked as a raw cancellation.
        var stub = new StubHttpMessageHandler((_, _) => throw new TaskCanceledException("timed out"));
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));

        var exception = await Assert.ThrowsAsync<TerminalApiException>(
            () => client.LoadTasksAsync(CancellationToken.None));
        Assert.Equal(TerminalApiErrorKind.Transport, exception.Kind);
    }

    [Fact]
    public async Task LoadTasksAsync_CallerCancellation_PropagatesOperationCanceled()
    {
        var stub = new StubHttpMessageHandler((_, token) =>
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(JsonResponse(HttpStatusCode.OK, "[]"));
        });
        var client = new TaskApiClient(CreateHttpClient(stub), DateProvider(new DateOnly(2026, 7, 23)));
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.LoadTasksAsync(cancellation.Token));
    }

    private static HttpClient CreateHttpClient(StubHttpMessageHandler stub, Func<string?>? tokenAccessor = null)
    {
        HttpMessageHandler handler = stub;
        if (tokenAccessor is not null)
        {
            handler = new TerminalHttpHandler(tokenAccessor, ClientId) { InnerHandler = stub };
        }

        return new HttpClient(handler) { BaseAddress = new Uri(BaseAddress) };
    }

    private static ILocalDateProvider DateProvider(DateOnly today)
    {
        var provider = new Mock<ILocalDateProvider>();
        provider.SetupGet(x => x.Today).Returns(today);
        return provider.Object;
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json)
    {
        return new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };
    }

    private static HttpResponseMessage TextResponse(HttpStatusCode status, string text)
    {
        return new HttpResponseMessage(status)
        {
            Content = new StringContent(text, Encoding.UTF8, "text/plain"),
        };
    }

    private static string CreateJwt(string username, DateTimeOffset expires)
    {
        var header = Base64Url("{\"alg\":\"HS256\",\"typ\":\"JWT\"}");
        var payload = Base64Url($"{{\"name\":\"{username}\",\"exp\":{expires.ToUnixTimeSeconds()}}}");
        return $"{header}.{payload}.{Base64Url("signature")}";
    }

    private static string Base64Url(string text)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string QueryParam(Uri uri, string name)
    {
        foreach (var pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=', StringComparison.Ordinal);
            if (separator > 0 && pair[..separator] == name)
            {
                return Uri.UnescapeDataString(pair[(separator + 1)..]);
            }
        }

        throw new InvalidOperationException($"Query parameter '{name}' was not found in '{uri.Query}'.");
    }

    private static List<JsonElement> EnumerateArray(JsonElement array)
    {
        return [.. array.EnumerateArray()];
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        public string? LastRequestBody { get; private set; }

        public MediaTypeHeaderValue? LastRequestContentType { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            LastRequestContentType = request.Content?.Headers.ContentType;
            if (request.Content is not null)
            {
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            return await responder(request, cancellationToken);
        }
    }
}
