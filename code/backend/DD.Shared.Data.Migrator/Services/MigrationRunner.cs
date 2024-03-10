using System.Diagnostics.CodeAnalysis;
using DD.Shared.Data.Migrator.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DD.Shared.Data.Migrator.Services;

internal class MigrationRunner(
    ILogger<MigrationRunner> logger,
    IMigratorMongoDbContext context,
    IServiceProvider serviceProvider) : IHostedService
{
    public const string MigrationsCollectionName = "_migrationsDomain";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return RunMigrationsAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task RunMigrationsAsync(CancellationToken cancellationToken)
    {
        Log.LookingForMigrations(logger);

        using var scope = serviceProvider.CreateScope();
        var migrationProvider = scope.ServiceProvider.GetRequiredService<MigrationProvider>();
        var migrationApplier = scope.ServiceProvider.GetRequiredService<MigrationApplier>();

        var migrations = migrationProvider.GetMigrationsToApply();

        Log.ApplyingMigrations(logger, migrations.Count);

        // await ApplyMigrationsInTransactionAsync(cancellationToken, migrations);
        await ApplyMigrationsAsync(migrationApplier, migrations, cancellationToken);

        Log.AppliedMigrations(logger, migrations.Count);
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "This method is commented out for now, but will be used in the future.")]
    private async Task ApplyMigrationsInTransactionAsync(MigrationApplier migrationApplier, IReadOnlyList<Migration> migrations, CancellationToken cancellationToken)
    {
        using var session = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);

        await session.WithTransactionAsync(
            async (_, _) =>
            {
                await ApplyMigrationsAsync(migrationApplier, migrations, cancellationToken);

                return 0;
            },
            cancellationToken: cancellationToken);
    }

    private static async Task ApplyMigrationsAsync(MigrationApplier migrationApplier, IReadOnlyList<Migration> migrations, CancellationToken cancellationToken)
    {
        foreach (var migration in migrations)
        {
            await migrationApplier.ApplyMigrationAsync(migration, cancellationToken);
        }
    }
}
