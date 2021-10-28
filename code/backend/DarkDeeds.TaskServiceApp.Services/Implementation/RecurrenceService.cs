﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.TaskServiceApp.EfCoreExtensions;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Models.Exceptions;
using DarkDeeds.TaskServiceApp.Models.Extensions;
using DarkDeeds.TaskServiceApp.Services.Interface;

namespace DarkDeeds.TaskServiceApp.Services.Implementation
{
    public class RecurrenceService : IRecurrenceService
    {
        private readonly IPlannedRecurrenceRepository _plannedRecurrenceRepository;
        private readonly IMapper _mapper;

        public RecurrenceService(IPlannedRecurrenceRepository plannedRecurrenceRepository, IMapper mapper)
        {
            _plannedRecurrenceRepository = plannedRecurrenceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlannedRecurrenceDto>> LoadAsync(string userId)
        {
            var recurrences = _plannedRecurrenceRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId));
            return (await _mapper.ProjectTo<PlannedRecurrenceDto>(recurrences).ToListSafeAsync())
                .ToUtcDate();
        }

        public async Task<int> SaveAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
        {   
            int[] ids = recurrences.Select(x => x.Id).ToArray();
            bool notUserEntities = await _plannedRecurrenceRepository.GetAll().AnySafeAsync(x =>
                !string.Equals(x.UserId, userId) &&
                ids.Contains(x.Id));

            if (notUserEntities)
                throw ServiceException.InvalidEntity("Recurrence");

            int count = 0;
            foreach (var dto in recurrences)
            {
                if (dto.IsDeleted)
                {
                    await _plannedRecurrenceRepository.DeleteAsync(dto.Id);
                    count++;
                }
                else if (dto.Id < 0)
                {
                    PlannedRecurrenceEntity entity = _mapper.Map<PlannedRecurrenceEntity>(dto);
                    entity.Id = 0;
                    entity.UserId = userId;
                    await _plannedRecurrenceRepository.SaveAsync(entity);
                    count++;
                }
                else
                {
                    PlannedRecurrenceEntity entity = await _plannedRecurrenceRepository.GetByIdAsync(dto.Id);
                    if (!RecurrenceIsChanged(entity, dto))
                        continue;
                    entity.Task = dto.Task;
                    entity.StartDate = dto.StartDate;
                    entity.EndDate = dto.EndDate;
                    entity.EveryNthDay = dto.EveryNthDay;
                    entity.EveryWeekday = dto.EveryWeekday;
                    entity.EveryMonthDay = dto.EveryMonthDay;
                    await _plannedRecurrenceRepository.SaveAsync(entity);
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