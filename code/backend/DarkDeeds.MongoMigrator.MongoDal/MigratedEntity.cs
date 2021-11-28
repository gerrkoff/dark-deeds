namespace DarkDeeds.MongoMigrator.MongoDal
{
    public class MigratedEntity
    {
        public string EntityUid { get; set; }
        public MigratedEntityType EntityType { get; set; }
    }

    public enum MigratedEntityType
    {
        Task, PlannedRecurrence, Recurrence
    }
}