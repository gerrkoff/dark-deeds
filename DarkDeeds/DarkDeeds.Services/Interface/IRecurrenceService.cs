using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models;

namespace DarkDeeds.Services.Interface
{
    public interface IRecurrenceService
    {
        Task<IEnumerable<PlannedRecurrenceDto>> GetRecurrences(string userId);
    }
}