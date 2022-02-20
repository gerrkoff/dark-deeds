using System;
using System.Threading.Tasks;
using DarkDeeds.MongoMigrator.PostgreDal.Models;
using DarkDeeds.MongoMigrator.PostgreDal.Repository;
using DarkDeeds.ServiceTask.Data.EntityRepository;
using DarkDeeds.ServiceTask.Enums;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.MongoMigrator.Processors
{
    class ProcessorTasks
    {
        private readonly Repository<TaskEntity> _pgTasks;
        private readonly TaskRepository _mongoTasks;
        private readonly bool _dryRun;
        private readonly Action<int> _progressNotifier;

        public ProcessorTasks(Repository<TaskEntity> pgTasks,
            TaskRepository mongoTasks,
            bool dryRun,
            Action<int> progressNotifier = null)
        {
            _pgTasks = pgTasks;
            _mongoTasks = mongoTasks;
            _dryRun = dryRun;
            _progressNotifier = progressNotifier;
        }

        public async Task<(int, int, int)> Process()
        {
            var total = 0;
            var created = 0;
            var updated = 0;
            var warning = 0;
            await foreach (var pgTask in _pgTasks.GetAll().AsAsyncEnumerable())
            {
                var mongoTask = await _mongoTasks.GetByIdAsync(pgTask.Uid);

                if (mongoTask == null)
                    created++;
                else
                {
                    updated++;
                    if (mongoTask.UserId != pgTask.UserId)
                        warning++;
                }

                if (!_dryRun)
                    await Upsert(pgTask);

                _progressNotifier?.Invoke(++total);
            }

            return (created, updated, warning);
        }

        private async Task Upsert(TaskEntity pgTask)
        {
            var mongoTask = new ServiceTask.Entities.TaskEntity
            {
                Uid = pgTask.Uid,
                IsDeleted =  pgTask.IsDeleted,
                Version = pgTask.Version,
                Title = pgTask.Title,
                Order = pgTask.Order,
                Date = pgTask.Date.HasValue ? DateTime.SpecifyKind(pgTask.Date.Value, DateTimeKind.Utc) : null,
                Type =(TaskTypeEnum) (int)pgTask.Type,
                Time = pgTask.Time,
                IsCompleted = pgTask.IsCompleted,
                IsProbable = pgTask.IsProbable,
                UserId = pgTask.UserId,
            };
            await _mongoTasks.UpsertAsync(mongoTask);
        }
    }
}
