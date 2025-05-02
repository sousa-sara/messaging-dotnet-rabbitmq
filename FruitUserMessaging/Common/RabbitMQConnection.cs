using System;
using RabbitMQ.Client;

namespace Common
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IModel GetChannel() => _channel;

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}