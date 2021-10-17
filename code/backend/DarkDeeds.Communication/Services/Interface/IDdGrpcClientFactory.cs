using System.Threading.Tasks;

namespace DarkDeeds.Communication.Services.Interface
{
    public interface IDdGrpcClientFactory<T>
    {
        Task<T> Create();
    }
}