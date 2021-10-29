using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.TaskServiceApp.EfCoreExtensions;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Infrastructure.Services.Dto;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.TaskServiceApp.Services.Implementation
{
    public class RecurrenceCreatorService : IRecurrenceCreatorService
    {
        private const int RecurrencePeriodInDays = 14;
        
        private readonly ITaskRepository _taskRepository;
        private readonly IPlannedRecurrenceRepository _plannedRecurrenceRepository;
        private readonly IDateService _dateService;
        private readonly ILogger<RecurrenceCreatorService> _logger;
        private readonly ITaskParserService _taskParserService;
        private readonly INotifierService _notifierService;
        private readonly IMapper _mapper;

        public RecurrenceCreatorService(
            ITaskRepository taskRepository,
            IPlannedRecurrenceRepository plannedRecurrenceRepository,
            IDateService dateService,
            ILogger<RecurrenceCreatorService> logger,
            ITaskParserService taskParserService,
            INotifierService notifierService, 
            IMapper mapper)
        {
            _taskRepository = taskRepository;
            _plannedRecurrenceRepository = plannedRecurrenceRepository;
            _dateService = dateService;
            _logger = logger;
            _taskParserService = taskParserService;
            _notifierService = notifierService;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(int timezoneOffset, string userId)
        {
            var createdRecurrencesCount = 0;
            List<PlannedRecurrenceEntity> plannedRecurrences = (await _plannedRecurrenceRepository
                .GetListAsync())
                .Where(x => string.Equals(x.UserId, userId))
                .ToList();

            foreach (PlannedRecurrenceEntity plannedRecurrence in plannedRecurrences)
            {
                ICollection<DateTime> dates = EvaluateRecurrenceDatesWithinPeriod(plannedRecurrence, timezoneOffset);
                foreach (var date in dates)
                {
                    bool alreadyExists = plannedRecurrence.Recurrences.Any(x => x.DateTime == date);
                    
                    if (alreadyExists)
                        continue;
                    
                    TaskEntity task = CreateTaskFromRecurrence(plannedRecurrence, date);
                    await _taskRepository.SaveAsync(task);
                    plannedRecurrence.Recurrences.Add(new RecurrenceEntity
                    {
                        DateTime = date,
                        TaskUid = task.Uid
                    });
                    await _plannedRecurrenceRepository.SaveRecurrences(plannedRecurrence);
                    createdRecurrencesCount++;
                    Notify(task, userId);
                }
            }

            return createdRecurrencesCount;
        }

        private void Notify(TaskEntity task, string userId)
        {
            var dto = _mapper.Map<TaskDto>(task);
            _notifierService.TaskUpdated(new TaskUpdatedDto
            {
                Tasks = new[] {dto},
                UserId = userId,
            });
        }

        private TaskEntity CreateTaskFromRecurrence(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
        {
            TaskDto dto = _taskParserService.ParseTask(plannedRecurrence.Task, ignoreDate: true);
            var task = _mapper.Map<TaskEntity>(dto);
            task.Date = date;
            task.UserId = plannedRecurrence.UserId;
            task.Uid = Guid.NewGuid().ToString();
            return task;
        }

        private ICollection<DateTime> EvaluateRecurrenceDatesWithinPeriod(PlannedRecurrenceEntity plannedRecurrence, int timezoneOffset)
        {
            var (periodStart, periodEnd) = EvaluatePeriod(timezoneOffset);
            var dates = new List<DateTime>();
            for (DateTime i = periodStart; i != periodEnd; i = i.AddDays(1))
            {
                if (!HasSchedule(plannedRecurrence) ||
                    !MatchPeriod(plannedRecurrence, i) ||
                    !MatchWeekday(plannedRecurrence, i) ||
                    !MatchNthDay(plannedRecurrence, i) ||
                    !MatchMonthDay(plannedRecurrence, i))
                    continue;
                
                dates.Add(i);
            }

            return dates;
        }

        private bool HasSchedule(PlannedRecurrenceEntity plannedRecurrence)
        {
            return plannedRecurrence.EveryNthDay.HasValue ||
                   plannedRecurrence.EveryWeekday.HasValue ||
                   !string.IsNullOrEmpty(plannedRecurrence.EveryMonthDay);
        }

        /// <remarks>End date is not included</remarks>
        public (DateTime, DateTime) EvaluatePeriod(int timezoneOffset)
        {
            var startDate = _dateService.Now.AddMinutes(timezoneOffset).Date;
            var currentDayOfWeek = (int) startDate.DayOfWeek;
            var currentDayOfWeekFixed = (6 + currentDayOfWeek) % 7 + 1;
            var endDate = startDate
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
                        $"Can't parse EveryMonthDay for PlannedRecurrenceUid = {plannedRecurrence.Uid}, Value = '{plannedRecurrence.EveryMonthDay}'");
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