using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return await _plannedRecurrenceRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .ProjectTo<PlannedRecurrenceDto>()
                .ToListSafeAsync();
        }

        public async Task<int> SaveAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
        {
            // TEST!
            await _permissionsService.CheckIfUserCanEditEntitiesAsync(
                recurrences.Cast<IDtoWithId>().ToList(),
                _plannedRecurrenceRepository,
                userId,
                "Recurrence");
            
            return 100500;
        }
    }
}