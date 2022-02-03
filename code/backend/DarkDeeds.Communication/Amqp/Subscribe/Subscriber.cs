using System;
using System.Threading.Tasks;
using DarkDeeds.Communication.Amqp.Common;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DarkDeeds.Communication.Amqp.Subscribe
{
    class Subscriber<T> : ISubscriber<T>
    {
        private readonly IChannelProvider _channelProvider;
        private readonly IAmqpItemSerializer _amqpItemSerializer;
        private readonly ILogger<Subscriber<T>> _logger;

        public Subscriber(IChannelProvider channelProvider, IAmqpItemSerializer amqpItemSerializer, ILogger<Subscriber<T>> logger)
        {
            _channelProvider = channelProvider;
            _amqpItemSerializer = amqpItemSerializer;
            _logger = logger;
        }
        
        public void Subscribe(string exchange, Func<T, Task> handler)
        {   
            var channel = _channelProvider.Provide();
            var queue = CreateSubscriberQueue(channel, exchange);
            var consumer = CreateConsumer(channel, handler);

            channel.BasicConsume(queue, true, consumer);
        }

        private string CreateSubscriberQueue(IModel channel, string exchange)
        {
            channel.ExchangeDeclare(exchange, ExchangeType.Fanout);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queueName, exchange, "");

            return queueName;
        }

        private IBasicConsumer CreateConsumer(IModel channel, Func<T, Task> handler)
        {
            var consumer = new EventingBasicConsumer(channel);

            async void OnConsumerOnReceived(object model, BasicDeliverEventArgs eventArgs)
            {
                var body = eventArgs.Body.ToArray();
                try
                {
                    var message = await _amqpItemSerializer.Deserialize<T>(body);
                    await handler(message);
                }
                catch (Exception e)
                {
                    // TODO: do we need this global try/catch?
                    _logger.LogWarning("Failed to process message, error: {0}", e);
                }
            }

            consumer.Received += OnConsumerOnReceived;
            return consumer;
        }

        public void Dispose()
        {
            _channelProvider?.Dispose();
        }
    }
}