using System;

namespace DarkDeeds.Communication.Amqp.Publish
{
    public interface IPublisher<T> : IDisposable
    {
        void Publish(PublishItem<T> item);
        string Exchange { set; }
    }
}