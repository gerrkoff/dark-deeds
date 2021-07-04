using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Services.Services.Interface
{
    public interface ITaskUpdatedListener
    {
        Task Process(ICollection<TaskDto> model);
    }
}