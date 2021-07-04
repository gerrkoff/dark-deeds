using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.Communication.Amqp.Subscribe
{
    public abstract class AbstractMessageSubscriber<T> : BackgroundService
    {
        private readonly string _exchange;
        private readonly ISubscriber<T> _subscriber;

        protected AbstractMessageSubscriber(string exchange, ISubscriber<T> subscriber)
        {
            _exchange = exchange;
            _subscriber = subscriber;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            
            _subscriber.Subscribe(_exchange, ProcessMessage);
            
            return Task.CompletedTask;
        }

        protected abstract Task ProcessMessage(T message);

        public override void Dispose()
        {
            base.Dispose();
            _subscriber?.Dispose();
        }
    }
}