using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models;

namespace DarkDeeds.Services.Interface
{
    public interface IRecurrenceService
    {
        Task<IEnumerable<PlannedRecurrenceDto>> LoadAsync(string userId);
        Task<int> SaveAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId);
    }
}