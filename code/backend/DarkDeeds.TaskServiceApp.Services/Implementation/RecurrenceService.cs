﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data.EntityRepository;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Models.Exceptions;
using DarkDeeds.TaskServiceApp.Models.Extensions;
using DarkDeeds.TaskServiceApp.Services.Interface;
using DarkDeeds.TaskServiceApp.Services.Specifications;

namespace DarkDeeds.TaskServiceApp.Services.Implementation
{
    public class RecurrenceService : IRecurrenceService
    {
        private readonly IPlannedRecurrenceRepository _plannedRecurrenceRepository;
        private readonly IMapper _mapper;
        private readonly ISpecificationFactory _specFactory;

        public RecurrenceService(IPlannedRecurrenceRepository plannedRecurrenceRepository, IMapper mapper, ISpecificationFactory specFactory)
        {
            _plannedRecurrenceRepository = plannedRecurrenceRepository;
            _mapper = mapper;
            _specFactory = specFactory;
        }

        public async Task<IEnumerable<PlannedRecurrenceDto>> LoadAsync(string userId)
        {
            var spec = _specFactory.New<IPlannedRecurrenceSpecification, PlannedRecurrenceEntity>()
                .FilterUserOwned(userId);

            var recurrences = await _plannedRecurrenceRepository.GetBySpecAsync(spec);

            return _mapper.Map<IList<PlannedRecurrenceDto>>(recurrences).ToUtcDate();
        }

        public async Task<int> SaveAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
        {
            string[] ids = recurrences.Select(x => x.Uid).ToArray();
            var foreignItemsSpec = _specFactory.New<IPlannedRecurrenceSpecification, PlannedRecurrenceEntity>()
                .FilterForeignUserOwned(userId, ids);
            bool notUserEntities = await _plannedRecurrenceRepository.AnyAsync(foreignItemsSpec);

            if (notUserEntities)
                throw ServiceException.InvalidEntity("Recurrence");

            int count = 0;
            foreach (var dto in recurrences)
            {
                if (dto.IsDeleted)
                {
                    await _plannedRecurrenceRepository.DeleteAsync(dto.Uid);
                    count++;
                    continue;
                }
                
                PlannedRecurrenceEntity entity = await _plannedRecurrenceRepository.GetByIdAsync(dto.Uid);
                if (entity == null)
                {
                    entity = _mapper.Map<PlannedRecurrenceEntity>(dto);
                    entity.UserId = userId;
                    await _plannedRecurrenceRepository.UpsertAsync(entity);
                    count++;
                }
                else
                {
                    if (!RecurrenceIsChanged(entity, dto))
                        continue;
                    entity.Task = dto.Task;
                    entity.StartDate = dto.StartDate;
                    entity.EndDate = dto.EndDate;
                    entity.EveryNthDay = dto.EveryNthDay;
                    entity.EveryWeekday = dto.EveryWeekday;
                    entity.EveryMonthDay = dto.EveryMonthDay;
                    var (success, _) = await _plannedRecurrenceRepository.TryUpdateVersionAsync(entity);
                    if (success)
                        count++;
                }
            }
            
            return count;
        }

        private bool RecurrenceIsChanged(PlannedRecurrenceEntity entity, PlannedRecurrenceDto dto)
        {
            return !string.Equals(entity.Task, dto.Task) ||
                   entity.StartDate != dto.StartDate ||
                   entity.EndDate != dto.EndDate ||
                   entity.EveryNthDay != dto.EveryNthDay ||
                   entity.EveryWeekday != dto.EveryWeekday ||
                   !string.Equals(entity.EveryMonthDay, dto.EveryMonthDay);
        }
    }
}