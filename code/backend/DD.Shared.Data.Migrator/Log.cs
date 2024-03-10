using Microsoft.Extensions.Logging;

namespace DD.Shared.Data.Migrator;

internal static partial class Log
{
    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Trace,
        Message = "Looking for migrations to apply...")]
    public static partial void LookingForMigrations(ILogger logger);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Debug,
        Message = "Applying {MigrationsCount} migrations...")]
    public static partial void ApplyingMigrations(ILogger logger, int migrationsCount);

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Information,
        Message = "Applied {MigrationsCount} migrations")]
    public static partial void AppliedMigrations(ILogger logger, int migrationsCount);

    [LoggerMessage(
        EventId = 3004,
        Level = LogLevel.Debug,
        Message = "Skipped {MigrationName} migration")]
    public static partial void SkippedMigration(ILogger logger, string migrationName);

    [LoggerMessage(
        EventId = 3005,
        Level = LogLevel.Debug,
        Message = "Found {MigrationName} migration to apply")]
    public static partial void FoundMigrationToApply(ILogger logger, string migrationName);

    [LoggerMessage(
        EventId = 3006,
        Level = LogLevel.Trace,
        Message = "Applying {MigrationName} migration...")]
    public static partial void ApplyingMigration(ILogger logger, string migrationName);

    [LoggerMessage(
        EventId = 3007,
        Level = LogLevel.Information,
        Message = "Applied {MigrationName} migration")]
    public static partial void AppliedMigration(ILogger logger, string migrationName);
}
