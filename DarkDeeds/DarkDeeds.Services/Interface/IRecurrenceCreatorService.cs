using System.Threading.Tasks;

namespace DarkDeeds.Services.Interface
{
    public interface IRecurrenceCreatorService
    {
        Task<int> CreateAsync(int timezoneOffset, string userId);
    }
}