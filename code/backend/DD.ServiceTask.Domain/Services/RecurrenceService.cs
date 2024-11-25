using AutoMapper;
using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.DtoExtensions;
using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Exceptions;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.ServiceTask.Domain.Specifications;

namespace DD.ServiceTask.Domain.Services;

public interface IRecurrenceService
{
    Task<IEnumerable<PlannedRecurrenceDto>> LoadAsync(string userId);

    Task<int> SaveAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId);
}

public class RecurrenceService(
    IPlannedRecurrenceRepository plannedRecurrenceRepository,
    IMapper mapper,
    ISpecificationFactory specFactory)
    : IRecurrenceService
{
    public async Task<IEnumerable<PlannedRecurrenceDto>> LoadAsync(string userId)
    {
        var spec = specFactory.Create<IPlannedRecurrenceSpecification, PlannedRecurrenceEntity>()
            .FilterUserOwned(userId)
            .FilterNotDeleted();

        var recurrences = await plannedRecurrenceRepository.GetBySpecAsync(spec);

        return mapper.Map<IList<PlannedRecurrenceDto>>(recurrences).ToUtcDate();
    }

    public async Task<int> SaveAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
    {
        await CheckIfUserOwns(recurrences, userId);

        var count = 0;
        foreach (var dto in recurrences)
        {
            if (await SavePlannedRecurrence(dto, userId))
                count++;
        }

        return count;
    }

    private async Task<bool> SavePlannedRecurrence(PlannedRecurrenceDto dto, string userId)
    {
        if (dto.IsDeleted)
        {
            await plannedRecurrenceRepository.DeleteAsync(dto.Uid);
            return true;
        }

        var entity = await plannedRecurrenceRepository.GetByIdAsync(dto.Uid);
        if (entity == null)
        {
            entity = mapper.Map<PlannedRecurrenceEntity>(dto);
            entity.UserId = userId;
            await plannedRecurrenceRepository.UpsertAsync(entity);
            return true;
        }

        if (!RecurrenceIsChanged(entity, dto))
            return false;

        entity.Task = dto.Task;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.EveryNthDay = dto.EveryNthDay;
        entity.EveryWeekday = dto.EveryWeekday;
        entity.EveryMonthDay = dto.EveryMonthDay;
        var (success, _) = await plannedRecurrenceRepository.TryUpdateVersionAsync(entity);
        return success;
    }

    private async Task CheckIfUserOwns(ICollection<PlannedRecurrenceDto> recurrences, string userId)
    {
        var ids = recurrences.Select(x => x.Uid).ToArray();
        var foreignItemsSpec = specFactory.Create<IPlannedRecurrenceSpecification, PlannedRecurrenceEntity>()
            .FilterForeignUserOwned(userId, ids);
        var notUserEntities = await plannedRecurrenceRepository.AnyAsync(foreignItemsSpec);

        if (notUserEntities)
            throw ServiceException.InvalidEntity("Recurrence");
    }

    private static bool RecurrenceIsChanged(PlannedRecurrenceEntity entity, PlannedRecurrenceDto dto)
    {
        return entity.Task != dto.Task ||
               entity.StartDate != dto.StartDate ||
               entity.EndDate != dto.EndDate ||
               entity.EveryNthDay != dto.EveryNthDay ||
               entity.EveryWeekday != dto.EveryWeekday ||
               entity.EveryMonthDay != dto.EveryMonthDay;
    }
}
