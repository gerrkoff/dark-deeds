using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace DarkDeeds.Communication.Amqp.Common
{
    class ChannelProvider : IChannelProvider
    {
        private readonly IConfiguration _configuration;

        private IConnection _connection;
        private IModel _channel;

        public ChannelProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IModel Provide()
        {
            CreateConnectionIfNot();

            if (_channel == null || !_channel.IsOpen)
                _channel = _connection.CreateModel();

            return _channel;
        }

        private void CreateConnectionIfNot()
        {
            if (_connection != null)
                return;
            
            var host = GetConnectionInfo();
            var factory = new ConnectionFactory {HostName = host};
            _connection = factory.CreateConnection();
        }

        private string GetConnectionInfo() => _configuration.GetConnectionString(Constants.ConnectionString);

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}