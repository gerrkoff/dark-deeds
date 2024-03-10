using DD.ServiceTask.Details.Data;
using DD.ServiceTask.Domain.Entities;
using DD.TelegramClient.Details.Data;
using DD.TelegramClient.Domain.Entities;
using DD.WebClientBff.Details.Data;
using DD.WebClientBff.Domain.Entities;
using MongoDB.Driver;

namespace DD.Shared.Data.Migrator.Migrations;

internal class D20240310N1_AddIndexesToEntities(
    TaskRepository taskRepository,
    PlannedRecurrenceRepository plannedRecurrenceRepository,
    UserSettingsRepository userSettingsRepository,
    TelegramUserRepository telegramUserRepository) : IMigrationBody
{
    public async Task UpAsync(CancellationToken cancellationToken)
    {
        var indexTasksUid = new CreateIndexModel<TaskEntity>(
            Builders<TaskEntity>.IndexKeys.Ascending(x => x.Uid),
            new CreateIndexOptions { Unique = true, Sparse = true });
        var indexTasksUserId = new CreateIndexModel<TaskEntity>(
            Builders<TaskEntity>.IndexKeys.Ascending(x => x.UserId),
            new CreateIndexOptions { Unique = false, Sparse = true });
        await taskRepository.Collection.Indexes.CreateManyAsync(
            [
                indexTasksUid,
                indexTasksUserId
            ],
            cancellationToken);

        var indexPlannedRecurrenceUid = new CreateIndexModel<PlannedRecurrenceEntity>(
            Builders<PlannedRecurrenceEntity>.IndexKeys.Ascending(x => x.Uid),
            new CreateIndexOptions { Unique = true, Sparse = true });
        var indexPlannedRecurrenceUserId = new CreateIndexModel<PlannedRecurrenceEntity>(
            Builders<PlannedRecurrenceEntity>.IndexKeys.Ascending(x => x.UserId),
            new CreateIndexOptions { Unique = false, Sparse = true });
        await plannedRecurrenceRepository.Collection.Indexes.CreateManyAsync(
            [
                indexPlannedRecurrenceUid,
                indexPlannedRecurrenceUserId
            ],
            cancellationToken);

        var indexUserSettingsUserId = new CreateIndexModel<UserSettingsEntity>(
            Builders<UserSettingsEntity>.IndexKeys.Ascending(x => x.UserId),
            new CreateIndexOptions { Unique = true, Sparse = true });
        await userSettingsRepository.Collection.Indexes.CreateManyAsync(
            [
                indexUserSettingsUserId
            ],
            cancellationToken);

        var indexTelegramUserUserId = new CreateIndexModel<TelegramUserEntity>(
            Builders<TelegramUserEntity>.IndexKeys.Ascending(x => x.UserId),
            new CreateIndexOptions { Unique = true, Sparse = true });
        await telegramUserRepository.Collection.Indexes.CreateManyAsync(
            [
                indexTelegramUserUserId
            ],
            cancellationToken);
    }
}
