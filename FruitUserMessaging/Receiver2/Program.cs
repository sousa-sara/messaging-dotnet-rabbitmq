using Common;
using Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receiver2
{
    class Program
    {
        static void Main(string[] args)
        {
            using var connection = new RabbitMQConnection();
            var channel = connection.GetChannel();

            // Declarar exchange e fila
            channel.ExchangeDeclare(exchange: "user_exchange", type: ExchangeType.Direct);
            channel.QueueDeclare(queue: "user_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: "user_queue", exchange: "user_exchange", routingKey: "user.receiver");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var user = JsonConvert.DeserializeObject<UserMessage>(message);
                Console.WriteLine($"[Receiver2] Usuário recebido: {user.FullName}, {user.CPF}, {user.RegistrationTimestamp}");
            };

            channel.BasicConsume(queue: "user_queue", autoAck: true, consumer: consumer);
            Console.WriteLine("[Receiver2] Aguardando mensagens...");
            Console.ReadLine();
        }
    }
}