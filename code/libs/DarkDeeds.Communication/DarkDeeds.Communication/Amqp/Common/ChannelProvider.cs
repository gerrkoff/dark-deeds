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
            
            var connectionInfo = GetConnectionInfo();
            var factory = new ConnectionFactory
            {
                HostName = connectionInfo.host,
                UserName = connectionInfo.user,
                Password = connectionInfo.pass,
            };
            _connection = factory.CreateConnection();
        }

        private (string host, string user, string pass) GetConnectionInfo()
        {
            var values = _configuration.GetConnectionString(Constants.ConnectionString).Split(";");
            return (values[0], values[1], values[2]);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}