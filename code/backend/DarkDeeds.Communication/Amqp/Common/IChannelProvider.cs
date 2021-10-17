using System;
using RabbitMQ.Client;

namespace DarkDeeds.Communication.Amqp.Common
{
    public interface IChannelProvider : IDisposable
    {
        IModel Provide();
    }
}