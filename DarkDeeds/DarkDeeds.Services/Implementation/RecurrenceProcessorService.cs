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
        private readonly IRepository<RecurrenceEntity> _recurrenceRepository;
        private readonly IRepositoryNonDeletable<RecurrenceTaskEntity> _recurrenceTaskRepository;
        private readonly IDateService _dateService;

        public RecurrenceProcessorService(
            IRepository<TaskEntity> taskRepository,
            IRepository<RecurrenceEntity> recurrenceRepository,
            IRepositoryNonDeletable<RecurrenceTaskEntity> recurrenceTaskRepository,
            IDateService dateService)
        {
            _taskRepository = taskRepository;
            _recurrenceRepository = recurrenceRepository;
            _recurrenceTaskRepository = recurrenceTaskRepository;
            _dateService = dateService;
        }

        public async Task CreateRecurrenceTasksAsync()
        {
            List<RecurrenceEntity> recurrences = await _recurrenceRepository.GetAll().ToListSafeAsync();
            foreach (RecurrenceEntity recurrence in recurrences)
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

        private TaskEntity CreateTaskFromRecurrence(RecurrenceEntity recurrence) => new TaskEntity
        {
            Title = recurrence.Task,
            UserId = recurrence.UserId,
        };

        private RecurrenceTaskEntity CreateRecurrentTaskCreatedEntity(RecurrenceEntity recurrence,
            TaskEntity task, DateTime date) => new RecurrenceTaskEntity
        {
            DateTime = date,
            RecurrenceId = recurrence.Id,
            TaskId = task.Id,
        };

        private ICollection<DateTime> ExpandRecurrenceWithinPeriod(RecurrenceEntity recurrence)
        {
            DateTime periodEnd = EvaluateRecurrencePeriodEndDate();
            var dates = new List<DateTime>();
            for (DateTime i = _dateService.Now; i.Date != periodEnd.Date; i = i.AddDays(1))
            {
                if (!MatchWeekday(recurrence, i))
                    continue;
                
                if (!MatchEveryNumberOfDays(recurrence, i))
                    continue;
                
                if (!MatchMonthDays(recurrence, i))
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

        public bool MatchEveryNumberOfDays(RecurrenceEntity recurrence, DateTime date)
        {
            if (recurrence.StartDate.Date != date.Date && recurrence.StartDate > date)
                return false;

            if (recurrence.EveryNumberOfDays == 0)
                return true;

            TimeSpan dayCount = recurrence.StartDate.Date - date.Date;
            return dayCount.Days % recurrence.EveryNumberOfDays == 0;
        }
        
        public bool MatchWeekday(RecurrenceEntity recurrence, DateTime date)
        {
            if (recurrence.Weekdays == null)
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
            
            return recurrence.Weekdays.Value.HasFlag(weekday);
        }
        
        public bool MatchMonthDays(RecurrenceEntity recurrence, DateTime date)
        {
            if (string.IsNullOrEmpty(recurrence.MonthDays))
                return true;

            int lastDay = DateTime.DaysInMonth(date.Year, date.Month);
            List<int> dayList = recurrence.MonthDays.Split(',').Select(int.Parse).ToList();

            if (dayList.Any(x => x > lastDay))
                dayList.Add(lastDay);

            return dayList.Contains(date.Day);
        }
    }
}