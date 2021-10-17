using System;
using System.Threading.Tasks;

namespace DarkDeeds.Communication.Amqp.Subscribe
{
    public interface ISubscriber<out T> : IDisposable
    {
        void Subscribe(string exchange, Func<T, Task> handler);
    }
}