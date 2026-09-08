using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using DD.Tests.Integration.Helpers;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using static DD.Tests.Integration.Helpers.Helper;

namespace DD.Tests.Integration.Infrastructure;

internal sealed class IntegrationEnvironment : IAsyncDisposable
{
    private const string MongoImage = "mongo:4.4";
    private const string ContainerNamePrefix = "dd-integration-tests-";

    private readonly MongoDbContainer _mongoContainer;
    private readonly DarkDeedsWebApplicationFactory _factory;
    private int _resourcesDisposed;

    private IntegrationEnvironment(
        MongoDbContainer mongoContainer,
        DarkDeedsWebApplicationFactory factory)
    {
        _mongoContainer = mongoContainer;
        _factory = factory;
    }

    internal static TimeSpan CleanupTimeout { get; } = TimeSpan.FromSeconds(30);

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _resourcesDisposed, 1) != 0)
            return;

        await DisposeResourcesAsync(_mongoContainer, _factory);
    }

    internal HttpClient CreateClient()
    {
        return _factory.CreateTestClient();
    }

    internal static async Task<IntegrationEnvironment> CreateAsync()
    {
        await DockerHelper.EnsureImageAsync(MongoImage);

        MongoDbContainer? mongoContainer = null;
        DarkDeedsWebApplicationFactory? factory = null;

        try
        {
            var containerName = $"{ContainerNamePrefix}{Guid.NewGuid():N}";
            mongoContainer = new MongoDbBuilder(MongoImage)
                .WithName(containerName)
                .Build();

            await mongoContainer.StartAsync();

            var databaseName = $"dd_integration_{Guid.NewGuid():N}";
            var databaseConnectionString = CreateDatabaseConnectionString(
                mongoContainer.GetConnectionString(),
                databaseName);
            factory = new DarkDeedsWebApplicationFactory(databaseConnectionString);

            using var client = factory.CreateTestClient();

            return new IntegrationEnvironment(mongoContainer, factory);
        }
        catch (Exception exception)
        {
            await DisposeResourcesAsync(mongoContainer, factory);
            ExceptionDispatchInfo.Capture(exception).Throw();
            throw;
        }
    }

    private static string CreateDatabaseConnectionString(string connectionString, string databaseName)
    {
        return new MongoUrlBuilder(connectionString)
        {
            AuthenticationSource = "admin",
            DatabaseName = databaseName,
        }.ToString();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Resource cleanup must attempt every teardown step and report failures without hiding the original startup failure.")]
    private static async Task DisposeResourcesAsync(
        MongoDbContainer? mongoContainer,
        DarkDeedsWebApplicationFactory? factory)
    {
        try
        {
            if (factory is not null)
            {
                await factory.DisposeAsync().AsTask().WaitAsync(CleanupTimeout);
            }
        }
        catch (Exception exception)
        {
            ReportCleanupFailure(exception);
        }
        finally
        {
            try
            {
                if (mongoContainer is not null)
                {
                    await mongoContainer.DisposeAsync().AsTask().WaitAsync(CleanupTimeout);
                }
            }
            catch (Exception exception)
            {
                ReportCleanupFailure(exception);
            }
        }
    }
}
