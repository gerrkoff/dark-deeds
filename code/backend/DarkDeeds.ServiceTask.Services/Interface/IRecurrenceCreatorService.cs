using System.Threading.Tasks;

namespace DarkDeeds.ServiceTask.Services.Interface
{
    public interface IRecurrenceCreatorService
    {
        Task<int> CreateAsync(int timezoneOffset, string userId);
    }
}