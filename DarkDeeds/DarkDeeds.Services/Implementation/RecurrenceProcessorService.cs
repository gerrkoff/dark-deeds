using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Enums;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class RecurrenceProcessorService : IRecurrenceProcessorService
    {
        private const int RecurrencePeriodInDays = 14;
        
        private readonly IRepository<TaskEntity> _taskRepository;
        private readonly IRepository<PlannedRecurrenceEntity> _recurrenceRepository;
        private readonly IRepositoryNonDeletable<RecurrenceEntity> _recurrenceTaskRepository;
        private readonly IDateService _dateService;

        public RecurrenceProcessorService(
            IRepository<TaskEntity> taskRepository,
            IRepository<PlannedRecurrenceEntity> recurrenceRepository,
            IRepositoryNonDeletable<RecurrenceEntity> recurrenceTaskRepository,
            IDateService dateService)
        {
            _taskRepository = taskRepository;
            _recurrenceRepository = recurrenceRepository;
            _recurrenceTaskRepository = recurrenceTaskRepository;
            _dateService = dateService;
        }

        public async Task CreateRecurrenceTasksAsync()
        {
            List<PlannedRecurrenceEntity> recurrences = await _recurrenceRepository.GetAll().ToListSafeAsync();
            foreach (PlannedRecurrenceEntity recurrence in recurrences)
            {
                ICollection<DateTime> expandedDates = ExpandRecurrenceWithinPeriod(recurrence);
                foreach (var date in expandedDates)
                {
                    TaskEntity task = CreateTaskFromRecurrence(recurrence);
                    await _taskRepository.SaveAsync(task);
                    await _recurrenceTaskRepository.SaveAsync(
                        CreateRecurrentTaskCreatedEntity(recurrence, task, date));
                }
            }
        }

        private TaskEntity CreateTaskFromRecurrence(PlannedRecurrenceEntity plannedRecurrence) => new TaskEntity
        {
            Title = plannedRecurrence.Task,
            UserId = plannedRecurrence.UserId,
        };

        private RecurrenceEntity CreateRecurrentTaskCreatedEntity(PlannedRecurrenceEntity plannedRecurrence,
            TaskEntity task, DateTime date) => new RecurrenceEntity
        {
            DateTime = date,
            PlannedRecurrenceId = plannedRecurrence.Id,
            TaskId = task.Id,
        };

        private ICollection<DateTime> ExpandRecurrenceWithinPeriod(PlannedRecurrenceEntity plannedRecurrence)
        {
            DateTime periodEnd = EvaluateRecurrencePeriodEndDate();
            var dates = new List<DateTime>();
            for (DateTime i = _dateService.Now; i.Date != periodEnd.Date; i = i.AddDays(1))
            {
                if (!MatchWeekday(plannedRecurrence, i))
                    continue;
                
                if (!MatchEveryNumberOfDays(plannedRecurrence, i))
                    continue;
                
                if (!MatchMonthDays(plannedRecurrence, i))
                    continue;
                
                dates.Add(i);
            }
            return dates;
        }

        /// <remarks>End date is not included</remarks>
        public DateTime EvaluateRecurrencePeriodEndDate()
        {
            int currentDayOfWeek = (int) _dateService.Now.DayOfWeek;
            return _dateService.Now.AddDays(RecurrencePeriodInDays - currentDayOfWeek + 1);
        }

        public bool MatchEveryNumberOfDays(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
        {
            if (plannedRecurrence.StartDate.Date != date.Date && plannedRecurrence.StartDate > date)
                return false;

            if (plannedRecurrence.EveryNthDay == 0)
                return true;

            TimeSpan dayCount = plannedRecurrence.StartDate.Date - date.Date;
            return dayCount.Days % plannedRecurrence.EveryNthDay == 0;
        }
        
        public bool MatchWeekday(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
        {
            if (plannedRecurrence.EveryWeekday == null)
                return true;
            
            var weekday = RecurrenceWeekdayEnum.None;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday: weekday = RecurrenceWeekdayEnum.Sunday; break;
                case DayOfWeek.Monday: weekday = RecurrenceWeekdayEnum.Monday; break;
                case DayOfWeek.Tuesday: weekday = RecurrenceWeekdayEnum.Tuesday; break;
                case DayOfWeek.Wednesday: weekday = RecurrenceWeekdayEnum.Wednesday; break;
                case DayOfWeek.Thursday: weekday = RecurrenceWeekdayEnum.Thursday; break;
                case DayOfWeek.Friday: weekday = RecurrenceWeekdayEnum.Friday; break;
                case DayOfWeek.Saturday: weekday = RecurrenceWeekdayEnum.Saturday; break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return plannedRecurrence.EveryWeekday.Value.HasFlag(weekday);
        }
        
        public bool MatchMonthDays(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
        {
            if (string.IsNullOrEmpty(plannedRecurrence.EveryMonthDay))
                return true;

            int lastDay = DateTime.DaysInMonth(date.Year, date.Month);
            List<int> dayList = plannedRecurrence.EveryMonthDay.Split(',').Select(int.Parse).ToList();

            if (dayList.Any(x => x > lastDay))
                dayList.Add(lastDay);

            return dayList.Contains(date.Day);
        }
    }
}