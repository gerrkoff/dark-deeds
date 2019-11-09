using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class RecurrenceService : IRecurrenceService
    {
        private readonly IRepository<PlannedRecurrenceEntity> _plannedRecurrenceRepository;

        public RecurrenceService(IRepository<PlannedRecurrenceEntity> plannedRecurrenceRepository)
        {
            _plannedRecurrenceRepository = plannedRecurrenceRepository;
        }

        public async Task<IEnumerable<PlannedRecurrenceDto>> LoadAsync(string userId)
        {
            return await _plannedRecurrenceRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .ProjectTo<PlannedRecurrenceDto>()
                .ToListSafeAsync();
        }

        public Task<int> SaveAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId)
        {
            return Task.FromResult(100500);
        }
    }
}