using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.Shared.Details.Data;
using MongoDB.Driver;

namespace DD.ServiceTask.Details.Data;

public sealed class PlannedRecurrenceRepository(IMongoDbContext dbContext)
    : Repository<PlannedRecurrenceEntity>(dbContext, "plannedRecurrences"), IPlannedRecurrenceRepository
{
    static PlannedRecurrenceRepository()
    {
        RegisterDefaultMap<PlannedRecurrenceEntity>();
        RegisterDefaultMap<RecurrenceEntity>();
    }

    public async Task<bool> TryAddRecurrenceAsync(string plannedRecurrenceUid, RecurrenceEntity recurrence)
    {
        // Filter: match PlannedRecurrence by UID and ensure the date doesn't already exist
        var filter = Builders<PlannedRecurrenceEntity>.Filter.And(
            Builders<PlannedRecurrenceEntity>.Filter.Eq(x => x.Uid, plannedRecurrenceUid),
            Builders<PlannedRecurrenceEntity>.Filter.Not(
                Builders<PlannedRecurrenceEntity>.Filter.ElemMatch(
                    x => x.Recurrences,
                    r => r.DateTime == recurrence.DateTime)));

        var update = Builders<PlannedRecurrenceEntity>.Update
            .Push(x => x.Recurrences, recurrence)
            .Inc(x => x.Version, 1);

        var result = await Collection.UpdateOneAsync(filter, update, new UpdateOptions
        {
            IsUpsert = false,
        });

        return result.ModifiedCount == 1;
    }

    public async Task<bool> TryUpdateRecurrenceTaskUidAsync(string plannedRecurrenceUid, DateTime dateTime, string taskUid)
    {
        // Filter: match PlannedRecurrence by UID and find the recurrence with the specified date
        var filter = Builders<PlannedRecurrenceEntity>.Filter.And(
            Builders<PlannedRecurrenceEntity>.Filter.Eq(x => x.Uid, plannedRecurrenceUid),
            Builders<PlannedRecurrenceEntity>.Filter.ElemMatch(
                x => x.Recurrences,
                r => r.DateTime == dateTime));

        var update = Builders<PlannedRecurrenceEntity>.Update.Set(
            $"{nameof(PlannedRecurrenceEntity.Recurrences)}.$.{nameof(RecurrenceEntity.TaskUid)}",
            taskUid);

        var result = await Collection.UpdateOneAsync(filter, update);

        return result.ModifiedCount == 1;
    }
}
