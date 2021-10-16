using System;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.Communication.Common;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;

namespace DarkDeeds.Communication.Amqp.Subscribe
{
    public abstract class AbstractMessageSubscriber<T> : AbstractRegisterBackgroundService<BrokerUnreachableException>, IDisposable
    {   
        private readonly string _exchange;
        private readonly ISubscriber<T> _subscriber;
        private readonly ILogger<AbstractMessageSubscriber<T>> _logger;

        protected AbstractMessageSubscriber(string exchange, ISubscriber<T> subscriber, ILogger<AbstractMessageSubscriber<T>> logger) :
            base(logger)
        {
            _exchange = exchange;
            _subscriber = subscriber;
            _logger = logger;
        }

        protected override Func<CancellationToken, Task> CreateRegisterJob() => cancellationToken =>
        {
            _subscriber.Subscribe(_exchange, ProcessMessage);
            _logger.LogInformation($"Subscribed to RabbitMQ '{_exchange}' successfully");
            return Task.CompletedTask;
        };
        
        protected abstract Task ProcessMessage(T message);
        
        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}