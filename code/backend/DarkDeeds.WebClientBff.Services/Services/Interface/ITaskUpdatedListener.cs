using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Services.Dto;

namespace DarkDeeds.WebClientBff.Services.Services.Interface
{
    public interface ITaskUpdatedListener
    {
        Task Process(TaskUpdatedDto model);
    }
}