using System;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.Common.Multithreading;
using DarkDeeds.Communication.Amqp.Common;
using RabbitMQ.Client;

namespace DarkDeeds.Communication.Amqp.Publish
{
    class Publisher<T> : ConcurrentQueueProcessor<PublishItem<T>>, IPublisher<T>
    {
        private readonly IAmqpItemSerializer _serializer;
        private readonly IChannelProvider _channelProvider;

        public Publisher(IChannelProvider channelProvider, IAmqpItemSerializer serializer)
        {
            _channelProvider = channelProvider;
            _serializer = serializer;
        }

        public void Publish(PublishItem<T> item) => Add(item);

        public string Exchange { private get; set; }

        protected override async Task Process(PublishItem<T> item, CancellationToken cancellationToken)
        {
            var msgSerialized = await _serializer.Serialize(item.Message);
            var channel = _channelProvider.Provide();
            var msgProps = channel.CreateBasicProperties();

            channel.BasicPublish(ExchangeWithCheck,
                item.RoutingKey,
                msgProps,
                msgSerialized);
        }

        protected override void OnQueueCreated()
        {
            var channel = _channelProvider.Provide();
            channel.ExchangeDeclare(ExchangeWithCheck, ExchangeType.Fanout);
        }

        private string ExchangeWithCheck
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Exchange))
                    throw new ArgumentNullException(nameof(Exchange), "Exchange must be set");
                return Exchange;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _channelProvider?.Dispose();
        }
    }
}