using System.Threading.Tasks;

namespace DarkDeeds.Services.Interface
{
    public interface IRecurrenceCreatorService
    {
        Task CreateAsync(string userId);
    }
}