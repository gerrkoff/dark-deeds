﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DarkDeeds.Common.Exceptions;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Entity.Base;
using DarkDeeds.Data.Repository;
using DarkDeeds.Models;
using DarkDeeds.Models.Data;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class RecurrenceService : IRecurrenceService
    {
        private readonly IRepository<PlannedRecurrenceEntity> _plannedRecurrenceRepository;
        private readonly IPermissionsService _permissionsService;

        public RecurrenceService(IRepository<PlannedRecurrenceEntity> plannedRecurrenceRepository, IPermissionsService permissionsService)
        {
            _plannedRecurrenceRepository = plannedRecurrenceRepository;
            _permissionsService = permissionsService;
        }

        public async Task<IEnumerable<PlannedRecurrenceDto>> LoadAsync(string userId)
        {
            return (await _plannedRecurrenceRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .ProjectTo<PlannedRecurrenceDto>()
                .ToListSafeAsync())
                .ToUtcDate();
        }

        public async Task<int> SaveAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
        {
            await _permissionsService.CheckIfUserCanEditEntitiesAsync(
                recurrences.Cast<IDtoWithId>().ToList(),
                _plannedRecurrenceRepository,
                userId,
                "Recurrence");

            // TODO!
            int count = 0;
            foreach (var dto in recurrences)
            {
                if (dto.IsDeleted)
                {
                    // TODO!
                    await _plannedRecurrenceRepository.DeleteAsync(dto.Id);
                    count++;
                }
                else if (dto.Id < 0)
                {
                    PlannedRecurrenceEntity entity = Mapper.Map<PlannedRecurrenceEntity>(dto);
                    // TODO!
                    await _plannedRecurrenceRepository.SaveAsync(entity);
                    count++;
                }
                else
                {
                    PlannedRecurrenceEntity entity = await _plannedRecurrenceRepository.GetByIdAsync(dto.Id);
                    // TODO!
                    if (!RecurrenceIsChanged(entity, dto))
                        continue;
                    // TODO!
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