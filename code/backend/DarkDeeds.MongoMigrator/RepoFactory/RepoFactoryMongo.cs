using DarkDeeds.ServiceTask.Data;
using DarkDeeds.ServiceTask.Data.EntityRepository;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.MongoMigrator.RepoFactory
{
    public class RepoFactoryMongo
    {
        public (TaskRepository, PlannedRecurrenceRepository) CreateRepos(IConfiguration configuration)
        {
            var context = new MongoDbContext(configuration.GetConnectionString("mongoDb"));
            var taskRepo = new TaskRepository(context);
            var plannedRecurrenceRepo = new PlannedRecurrenceRepository(context);
            return (taskRepo, plannedRecurrenceRepo);
        }
    }
}