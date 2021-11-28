using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.MongoMigrator.PostgreDal.Models;
using DarkDeeds.MongoMigrator.PostgreDal.Repository;
using DarkDeeds.TaskServiceApp.Data.EntityRepository;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.MongoMigrator.Processors
{
    public class ProcessorRecurrences
    {
        private readonly RepositoryNonDeletable<RecurrenceEntity> _pgRecurrences;
        private readonly PlannedRecurrenceRepository _mongoPlannedRecurrences;
        private readonly bool _dryRun;
        private readonly Action<int> _progressNotifier;

        public ProcessorRecurrences(RepositoryNonDeletable<RecurrenceEntity> pgRecurrences,
            PlannedRecurrenceRepository mongoPlannedRecurrences,
            bool dryRun,
            Action<int> progressNotifier)
        {
            _pgRecurrences = pgRecurrences;
            _mongoPlannedRecurrences = mongoPlannedRecurrences;
            _dryRun = dryRun;
            _progressNotifier = progressNotifier;
        }

        public async Task<(int, int, int)> Process()
        {
            var total = 0;
            var created = 0;
            var skipped = 0;
            var warning = 0;
            await foreach (var pgRecurrence in _pgRecurrences.GetAll().AsAsyncEnumerable())
            {
                var mongoPlannedRecurrence = await _mongoPlannedRecurrences.GetByIdAsync(pgRecurrence.PlannedRecurrenceId.ToString());

                if (mongoPlannedRecurrence == null)
                {
                    warning++;
                }
                else if (mongoPlannedRecurrence.Recurrences.All(x => x.TaskUid != pgRecurrence.Task.Uid))
                {
                    created++;
                    
                    if (!_dryRun)
                        await Upsert(pgRecurrence, mongoPlannedRecurrence);
                }
                else
                {
                    skipped++;
                }

                _progressNotifier?.Invoke(++total);
            }

            return (created, skipped, warning);
        }

        private async Task Upsert(RecurrenceEntity pgRecurrence, TaskServiceApp.Entities.Models.PlannedRecurrenceEntity mongoPlannedRecurrence)
        {
            mongoPlannedRecurrence.Recurrences.Add(new TaskServiceApp.Entities.Models.RecurrenceEntity
            {
                TaskUid = pgRecurrence.Task.Uid,
                DateTime = DateTime.SpecifyKind(pgRecurrence.DateTime, DateTimeKind.Utc),
            });
            await _mongoPlannedRecurrences.UpsertAsync(mongoPlannedRecurrence);
        }
    }
}