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

        public async Task<IEnumerable<PlannedRecurrenceDto>> GetRecurrences(string userId)
        {
            return await _plannedRecurrenceRepository.GetAll()
                .Where(x => string.Equals(x.UserId, userId))
                .ProjectTo<PlannedRecurrenceDto>()
                .ToListSafeAsync();
        }
    }
}