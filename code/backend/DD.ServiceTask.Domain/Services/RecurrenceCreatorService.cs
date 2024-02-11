using AutoMapper;
using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Enums;
using DD.ServiceTask.Domain.Infrastructure;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.ServiceTask.Domain.Specifications;
using Microsoft.Extensions.Logging;

namespace DD.ServiceTask.Domain.Services;

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

        var spec = specFactory.Create<IPlannedRecurrenceSpecification, PlannedRecurrenceEntity>()
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

                var task = CreateTaskFromRecurrence(plannedRecurrence, date);
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
        var entityToSave = entity;
        bool success;
        do
        {
            entity.Recurrences.Add(recurrence);
            (success, var currentEntity) = await plannedRecurrenceRepository
                .TryUpdateVersionPropsAsync(entityToSave, x => x.Recurrences);
            if (!success)
                entityToSave = currentEntity;
        } while (!success && entityToSave != null);

        return entityToSave ?? throw new InvalidOperationException("Can't save recurrence");
    }

    private void Notify(TaskEntity task, string userId)
    {
        var dto = mapper.Map<TaskDto>(task);
        notifierService.TaskUpdated(new TaskUpdatedDto
        {
            Tasks = new[] { dto },
            UserId = userId,
        });
    }

    private TaskEntity CreateTaskFromRecurrence(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
    {
        var dto = taskParserService.ParseTask(plannedRecurrence.Task, ignoreDate: true);
        var task = mapper.Map<TaskEntity>(dto);
        task.Date = date;
        task.UserId = plannedRecurrence.UserId;
        task.Uid = Guid.NewGuid().ToString();
        return task;
    }

    private List<DateTime> EvaluateRecurrenceDatesWithinPeriod(PlannedRecurrenceEntity plannedRecurrence, int timezoneOffset)
    {
        var (periodStart, periodEnd) = EvaluatePeriod(timezoneOffset);
        var dates = new List<DateTime>();
        for (var i = periodStart; i != periodEnd; i = i.AddDays(1))
        {
            if (!HasSchedule(plannedRecurrence) ||
                !MatchPeriod(plannedRecurrence, i) ||
                !MatchWeekday(plannedRecurrence, i) ||
                !MatchNthDay(plannedRecurrence, i) ||
                !MatchMonthDay(plannedRecurrence, i))
            {
                continue;
            }

            dates.Add(i);
        }

        return dates;
    }

    private static bool HasSchedule(PlannedRecurrenceEntity plannedRecurrence)
    {
        return plannedRecurrence.EveryNthDay.HasValue ||
               plannedRecurrence.EveryWeekday.HasValue ||
               !string.IsNullOrEmpty(plannedRecurrence.EveryMonthDay);
    }

    /// <remarks>End date is not included</remarks>
    public (DateTime, DateTime) EvaluatePeriod(int timezoneOffset)
    {
        var startDate = dateService.Now.AddMinutes(timezoneOffset).Date;
        var currentDayOfWeek = (int)startDate.DayOfWeek;
        var currentDayOfWeekFixed = (6 + currentDayOfWeek) % 7 + 1;
        var endDate = startDate
            .AddDays(RecurrencePeriodInDays - currentDayOfWeekFixed + 1)
            .Date;

        return (startDate, endDate);
    }

    public static bool MatchPeriod(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
    {
        return date >= plannedRecurrence.StartDate &&
               (!plannedRecurrence.EndDate.HasValue || date <= plannedRecurrence.EndDate);
    }

    public static bool MatchNthDay(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
    {
        if (!plannedRecurrence.EveryNthDay.HasValue)
            return true;

        var dayCount = plannedRecurrence.StartDate.Date - date.Date;
        return dayCount.Days % plannedRecurrence.EveryNthDay == 0;
    }

    public static bool MatchWeekday(PlannedRecurrenceEntity plannedRecurrence, DateTime date)
    {
        if (!plannedRecurrence.EveryWeekday.HasValue)
            return true;
        var weekday = date.DayOfWeek switch
        {
            DayOfWeek.Sunday => RecurrenceWeekday.Sunday,
            DayOfWeek.Monday => RecurrenceWeekday.Monday,
            DayOfWeek.Tuesday => RecurrenceWeekday.Tuesday,
            DayOfWeek.Wednesday => RecurrenceWeekday.Wednesday,
            DayOfWeek.Thursday => RecurrenceWeekday.Thursday,
            DayOfWeek.Friday => RecurrenceWeekday.Friday,
            DayOfWeek.Saturday => RecurrenceWeekday.Saturday,
            _ => throw new ArgumentOutOfRangeException(nameof(date)),
        };
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
            if (e is not (OverflowException or FormatException)) throw;

            Log.FailedToParseMonthWorkDay(logger, plannedRecurrence.Uid, plannedRecurrence.EveryMonthDay);

            return true;
        }

        var lastDay = DateTime.DaysInMonth(date.Year, date.Month);


        if (dayList.Any(x => x > lastDay))
            dayList.Add(lastDay);

        return dayList.Contains(date.Day);
    }
}
