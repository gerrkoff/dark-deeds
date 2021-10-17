using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DarkDeeds.Communication.Amqp.Common
{
    // TODO: refactor ?
    class AmqpItemSerializer : IAmqpItemSerializer
    {
        public async Task<byte[]> Serialize<T>(T item)
        {
            var serialized = await HttpExtensions.SerializeAsync(item);
            return Encoding.UTF8.GetBytes(serialized);
        }

        public Task<T> Deserialize<T>(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            return HttpExtensions.DeserializeAsync<T>(stream);
        }
    }
}