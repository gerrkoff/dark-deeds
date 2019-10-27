using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Enums;
using DarkDeeds.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.Services.Implementation
{
    public class RecurrenceCreatorService : IRecurrenceCreatorService
    {
        private const int RecurrencePeriodInDays = 14;
        
        private readonly IRepository<TaskEntity> _taskRepository;
        private readonly IRepository<PlannedRecurrenceEntity> _plannedRecurrenceRepository;
        private readonly IRepositoryNonDeletable<RecurrenceEntity> _recurrenceRepository;
        private readonly IDateService _dateService;
        private readonly ILogger<RecurrenceCreatorService> _logger;

        public RecurrenceCreatorService(
            IRepository<TaskEntity> taskRepository,
            IRepository<PlannedRecurrenceEntity> plannedRecurrenceRepository,
            IRepositoryNonDeletable<RecurrenceEntity> recurrenceRepository,
            IDateService dateService,
            ILogger<RecurrenceCreatorService> logger)
        {
            _taskRepository = taskRepository;
            _plannedRecurrenceRepository = plannedRecurrenceRepository;
            _recurrenceRepository = recurrenceRepository;
            _dateService = dateService;
            _logger = logger;
        }

        public async Task CreateAsync(string userId)
        {
            List<PlannedRecurrenceEntity> plannedRecurrences = await _plannedRecurrenceRepository
                .GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .ToListSafeAsync();

            foreach (PlannedRecurrenceEntity plannedRecurrence in plannedRecurrences)
            {
                ICollection<DateTime> dates = EvaluateRecurrenceDatesWithinPeriod(plannedRecurrence);
                foreach (var date in dates)
                {
                    bool alreadyExists = await _recurrenceRepository.GetAll().AnySafeAsync(
                        x => x.PlannedRecurrenceId == plannedRecurrence.Id && x.DateTime == date);
                    
                    if (alreadyExists)
                        continue;;
                    
                    TaskEntity task = CreateTaskFromRecurrence(plannedRecurrence, date);
                    await _taskRepository.SaveAsync(task);
                    // TODO: notify about task created
                    await _recurrenceRepository.SaveAsync(
                        CreateRecurrenceEntity(plannedRecurrence.Id, task.Id, date));
                }
            }
        }

        // TODO: parse task text
        private TaskEntity CreateTaskFromRecurrence(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
            => new TaskEntity
            {
                Title = plannedRecurrence.Task,
                UserId = plannedRecurrence.UserId,
                Date = date,
            };

        private RecurrenceEntity CreateRecurrenceEntity(int plannedRecurrenceId, int taskId, DateTime date)
            => new RecurrenceEntity
            {
                DateTime = date,
                PlannedRecurrenceId = plannedRecurrenceId,
                TaskId = taskId,
            };

        private ICollection<DateTime> EvaluateRecurrenceDatesWithinPeriod(PlannedRecurrenceEntity plannedRecurrence)
        {
            var (periodStart, periodEnd) = EvaluatePeriod();
            var dates = new List<DateTime>();
            for (DateTime i = periodStart; i != periodEnd; i = i.AddDays(1))
            {
                if (!MatchPeriod(plannedRecurrence, i))
                    continue;

                if (!MatchWeekday(plannedRecurrence, i))
                    continue;
                
                if (!MatchNthDay(plannedRecurrence, i))
                    continue;
                
                if (!MatchMonthDay(plannedRecurrence, i))
                    continue;
                
                dates.Add(i);
            }
            return dates;
        }

        /// <remarks>End date is not included</remarks>
        public (DateTime, DateTime) EvaluatePeriod()
        {
            var startDate = _dateService.Now.AddHours(-12).Date;

            var startDateForEndDate = _dateService.Now.AddHours(12);
            var currentDayOfWeek = (int) startDateForEndDate.DayOfWeek;
            var currentDayOfWeekFixed = (6 + currentDayOfWeek) % 7 + 1;
            var endDate = startDateForEndDate
                .AddDays(RecurrencePeriodInDays - currentDayOfWeekFixed + 1)
                .Date;
            
            return (startDate, endDate);
        }

        public bool MatchPeriod(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
        {
            return date >= plannedRecurrence.StartDate &&
                   (!plannedRecurrence.EndDate.HasValue || date <= plannedRecurrence.EndDate);
        }

        public bool MatchNthDay(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
        {
            if (!plannedRecurrence.EveryNthDay.HasValue)
                return true;

            TimeSpan dayCount = plannedRecurrence.StartDate.Date - date.Date;
            return dayCount.Days % plannedRecurrence.EveryNthDay == 0;
        }
        
        public bool MatchWeekday(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
        {
            if (!plannedRecurrence.EveryWeekday.HasValue)
                return true;
            
            RecurrenceWeekdayEnum weekday;
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
        
        public bool MatchMonthDay(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
        {
            if (string.IsNullOrEmpty(plannedRecurrence.EveryMonthDay))
                return true;

            List<int> dayList;
            try
            {
                dayList = plannedRecurrence.EveryMonthDay.Split(',').Select(int.Parse).ToList();
            }
            catch (Exception e)
            {
                if (e is OverflowException || e is FormatException)
                {
                    _logger.LogWarning(
                        $"Can't parse EveryMonthDay for PlannedRecurrenceId = {plannedRecurrence.Id}, Value = '{plannedRecurrence.EveryMonthDay}'");
                    return true;
                }

                throw;
            }

            int lastDay = DateTime.DaysInMonth(date.Year, date.Month);
            

            if (dayList.Any(x => x > lastDay))
                dayList.Add(lastDay);

            return dayList.Contains(date.Day);
        }
    }
}