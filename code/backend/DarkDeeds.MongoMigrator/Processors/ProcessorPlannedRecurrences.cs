using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.MongoMigrator.PostgreDal.Models;
using DarkDeeds.MongoMigrator.PostgreDal.Repository;
using DarkDeeds.ServiceTask.Data.EntityRepository;
using DarkDeeds.ServiceTask.Enums;
using Microsoft.EntityFrameworkCore;
using RecurrenceEntity = DarkDeeds.ServiceTask.Entities.RecurrenceEntity;

namespace DarkDeeds.MongoMigrator.Processors
{
    class ProcessorPlannedRecurrences
    {
        private readonly Repository<PlannedRecurrenceEntity> _pgPlannedRecurrences;
        private readonly PlannedRecurrenceRepository _mongoPlannedRecurrences;
        private readonly bool _dryRun;
        private readonly Action<int> _progressNotifier;

        public ProcessorPlannedRecurrences(Repository<PlannedRecurrenceEntity> pgPlannedRecurrences, PlannedRecurrenceRepository mongoPlannedRecurrences, bool dryRun, Action<int> progressNotifier)
        {
            _pgPlannedRecurrences = pgPlannedRecurrences;
            _mongoPlannedRecurrences = mongoPlannedRecurrences;
            _dryRun = dryRun;
            _progressNotifier = progressNotifier;
        }

        public async Task<(int, int, int)> Process()
        {
            var total = 0;
            var created = 0;
            var updated = 0;
            var warning = 0;
            await foreach (var pgPlannedRecurrence in _pgPlannedRecurrences.GetAll().AsAsyncEnumerable())
            {
                var mongoPlannedRecurrence = await _mongoPlannedRecurrences.GetByIdAsync(pgPlannedRecurrence.Id.ToString());

                if (mongoPlannedRecurrence == null)
                    created++;
                else
                {
                    updated++;
                    if (mongoPlannedRecurrence.UserId != pgPlannedRecurrence.UserId)
                        warning++;
                }

                if (!_dryRun)
                    await Upsert(pgPlannedRecurrence);

                _progressNotifier?.Invoke(++total);
            }

            return (created, updated, warning);
        }

        private async Task Upsert(PlannedRecurrenceEntity pgPlannedRecurrence)
        {
            var mongoPlannedRecurrence = new ServiceTask.Entities.PlannedRecurrenceEntity()
            {
                Task = pgPlannedRecurrence.Task,
                StartDate = DateTime.SpecifyKind(pgPlannedRecurrence.StartDate, DateTimeKind.Utc),
                EndDate = pgPlannedRecurrence.EndDate.HasValue ? DateTime.SpecifyKind(pgPlannedRecurrence.EndDate.Value, DateTimeKind.Utc) : null,
                EveryNthDay = pgPlannedRecurrence.EveryNthDay,
                EveryMonthDay = pgPlannedRecurrence.EveryMonthDay,
                EveryWeekday = (RecurrenceWeekdayEnum?) (int?) pgPlannedRecurrence.EveryWeekday,
                UserId = pgPlannedRecurrence.UserId,
                Recurrences = new List<RecurrenceEntity>(),
                Uid = pgPlannedRecurrence.Id.ToString(),
                IsDeleted = pgPlannedRecurrence.IsDeleted,
                Version = 1,
            };
            await _mongoPlannedRecurrences.UpsertAsync(mongoPlannedRecurrence);
        }
    }
}
