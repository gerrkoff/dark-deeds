namespace DarkDeeds.Communication.Amqp.Publish
{
    public class PublishItem<T>
    {
        public PublishItem(T message, string routingKey = "")
        {
            RoutingKey = routingKey;
            Message = message;
        }

        public string RoutingKey { get; }
        public T Message { get; }
    }
}