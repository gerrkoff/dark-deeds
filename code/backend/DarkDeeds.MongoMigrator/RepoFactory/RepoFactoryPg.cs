using DarkDeeds.MongoMigrator.PostgreDal.Context;
using DarkDeeds.MongoMigrator.PostgreDal.Models;
using DarkDeeds.MongoMigrator.PostgreDal.Repository;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.MongoMigrator.RepoFactory
{
    public class RepoFactoryPg
    {
        public (Repository<TaskEntity>, Repository<PlannedRecurrenceEntity>, RepositoryNonDeletable<RecurrenceEntity>) CreateRepos(IConfiguration configuration)
        {
            var context = new ContextDesignTimeFactory().CreateDbContext(configuration);
            var taskRepo = new Repository<TaskEntity>(context);
            var plannedRecurrenceRepo = new Repository<PlannedRecurrenceEntity>(context);
            var recurrenceRepo = new RepositoryNonDeletable<RecurrenceEntity>(context);
            return (taskRepo, plannedRecurrenceRepo, recurrenceRepo);
        }
    }
}