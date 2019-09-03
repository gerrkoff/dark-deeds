using System.Threading.Tasks;

namespace DarkDeeds.Services.Interface
{
    public interface IRecurrenceProcessorService
    {
        Task CreateRecurrenceTasksAsync();
    }
}