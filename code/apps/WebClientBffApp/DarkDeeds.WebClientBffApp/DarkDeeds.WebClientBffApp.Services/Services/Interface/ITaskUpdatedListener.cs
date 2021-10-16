using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Services.Dto;

namespace DarkDeeds.WebClientBffApp.Services.Services.Interface
{
    public interface ITaskUpdatedListener
    {
        Task Process(TaskUpdatedDto model);
    }
}