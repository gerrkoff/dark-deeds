using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models.Dto;

namespace DarkDeeds.Infrastructure.Services
{
    public interface ITaskHubService
    {
        Task Update(IEnumerable<TaskDto> updatedTasks);
    }
}