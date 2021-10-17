using System.Threading.Tasks;

namespace DarkDeeds.Communication.Amqp.Common
{
    // TODO: IAmqpMessageSerializer
    public interface IAmqpItemSerializer
    {
        Task<byte[]> Serialize<T>(T item);
        Task<T> Deserialize<T>(byte[] bytes);
    }
}