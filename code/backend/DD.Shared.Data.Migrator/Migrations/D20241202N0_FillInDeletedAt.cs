using DD.ServiceTask.Details.Data;
using DD.ServiceTask.Domain.Entities;
using DD.Shared.Data.Migrator.Models;
using MongoDB.Driver;

namespace DD.Shared.Data.Migrator.Migrations;

internal sealed class D20241202N0_FillInDeletedAt(
    TaskRepository taskRepository,
    PlannedRecurrenceRepository plannedRecurrenceRepository) : IMigrationBody
{
    public async Task UpAsync(CancellationToken cancellationToken)
    {
        await taskRepository.Collection.UpdateManyAsync(
            Builders<TaskEntity>.Filter.Eq("IsDeleted", true),
            Builders<TaskEntity>.Update.Set(x => x.DeletedAt, DateTimeOffset.UtcNow.AddDays(-30)),
            cancellationToken: cancellationToken);

        await plannedRecurrenceRepository.Collection.UpdateManyAsync(
            Builders<PlannedRecurrenceEntity>.Filter.Eq("IsDeleted", true),
            Builders<PlannedRecurrenceEntity>.Update.Set(x => x.DeletedAt, DateTimeOffset.UtcNow.AddDays(-30)),
            cancellationToken: cancellationToken);
    }
}
