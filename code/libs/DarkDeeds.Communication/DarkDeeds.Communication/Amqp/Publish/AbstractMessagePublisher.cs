using System;

namespace DarkDeeds.Communication.Amqp.Publish
{
    public abstract class AbstractMessagePublisher<T> : IDisposable
    {
        private readonly IPublisher<T> _publisher;

        protected AbstractMessagePublisher(string exchange, IPublisher<T> publisher)
        {
            _publisher = publisher;
            _publisher.Exchange = exchange;
        }

        protected void Publish(PublishItem<T> item) => _publisher.Publish(item);

        public void Dispose()
        {
            _publisher?.Dispose();
        }
    }
}