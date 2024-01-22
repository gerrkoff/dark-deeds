using AutoMapper;
using DD.TaskService.Domain.Dto;
using DD.TaskService.Domain.Entities;
using DD.TaskService.Domain.Entities.Enums;
using DD.TaskService.Domain.Infrastructure;
using DD.TaskService.Domain.Infrastructure.EntityRepository;
using DD.TaskService.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace DD.TaskService.Domain.Services;

public interface IRecurrenceCreatorService
{
    Task<int> CreateAsync(int timezoneOffset, string userId);
}

public class RecurrenceCreatorService(
    ITaskRepository taskRepository,
    IPlannedRecurrenceRepository plannedRecurrenceRepository,
    IDateService dateService,
    ILogger<RecurrenceCreatorService> logger,
    ITaskParserService taskParserService,
    INotifierService notifierService,
    IMapper mapper,
    ISpecificationFactory specFactory)
    : IRecurrenceCreatorService
{
    private const int RecurrencePeriodInDays = 14;

    public async Task<int> CreateAsync(int timezoneOffset, string userId)
    {
        var createdRecurrencesCount = 0;

        var spec = specFactory.New<IPlannedRecurrenceSpecification, PlannedRecurrenceEntity>()
            .FilterUserOwned(userId)
            .FilterNotDeleted();
        var plannedRecurrences = await plannedRecurrenceRepository.GetBySpecAsync(spec);

        foreach (var item in plannedRecurrences)
        {
            var plannedRecurrence = item;
            var dates = EvaluateRecurrenceDatesWithinPeriod(plannedRecurrence, timezoneOffset);
            foreach (var date in dates)
            {
                var alreadyExists = plannedRecurrence.Recurrences.Any(x => x.DateTime == date);

                if (alreadyExists)
                    continue;

                TaskEntity task = CreateTaskFromRecurrence(plannedRecurrence, date);
                await taskRepository.UpsertAsync(task);
                plannedRecurrence = await SaveRecurrence(plannedRecurrence,
                    new RecurrenceEntity
                    {
                        DateTime = date,
                        TaskUid = task.Uid
                    });
                createdRecurrencesCount++;
                Notify(task, userId);
            }
        }

        return createdRecurrencesCount;
    }

    private async Task<PlannedRecurrenceEntity> SaveRecurrence(PlannedRecurrenceEntity entity,
        RecurrenceEntity recurrence)
    {
        bool success;
        do
        {
            entity.Recurrences.Add(recurrence);
            PlannedRecurrenceEntity currentEntity;
            (success, currentEntity) = await plannedRecurrenceRepository
                .TryUpdateVersionPropsAsync(entity, x => x.Recurrences);
            if (!success)
                entity = currentEntity;
        } while (!success);

        return entity;
    }

    private void Notify(TaskEntity task, string userId)
    {
        var dto = mapper.Map<TaskDto>(task);
        notifierService.TaskUpdated(new TaskUpdatedDto
        {
            Tasks = new[] {dto},
            UserId = userId,
        });
    }

    private TaskEntity CreateTaskFromRecurrence(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
    {
        TaskDto dto = taskParserService.ParseTask(plannedRecurrence.Task, ignoreDate: true);
        var task = mapper.Map<TaskEntity>(dto);
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
        var startDate = dateService.Now.AddMinutes(timezoneOffset).Date;
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
                logger.LogWarning(
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
