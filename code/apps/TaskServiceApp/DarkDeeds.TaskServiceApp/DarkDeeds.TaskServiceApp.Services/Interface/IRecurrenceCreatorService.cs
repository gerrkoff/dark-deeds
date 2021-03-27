using System.Threading.Tasks;

namespace DarkDeeds.TaskServiceApp.Services.Interface
{
    public interface IRecurrenceCreatorService
    {
        Task<int> CreateAsync(int timezoneOffset, string userId);
    }
}