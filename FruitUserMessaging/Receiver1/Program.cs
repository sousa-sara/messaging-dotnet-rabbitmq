using Common;
using Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Receiver1
{
    class Program
    {
        static void Main(string[] args)
        {
            using var connection = new RabbitMQConnection();
            var channel = connection.GetChannel();

            // Declarar exchange e fila
            channel.ExchangeDeclare(exchange: "fruit_exchange", type: ExchangeType.Direct);
            channel.QueueDeclare(queue: "fruit_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: "fruit_queue", exchange: "fruit_exchange", routingKey: "fruit.receiver");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var fruit = JsonConvert.DeserializeObject<FruitMessage>(message);
                Console.WriteLine($"[Receiver1] Fruta recebida: {fruit.FruitName}, {fruit.Description}, {fruit.Timestamp}");
            };

            channel.BasicConsume(queue: "fruit_queue", autoAck: true, consumer: consumer);
            Console.WriteLine("[Receiver1] Aguardando mensagens...");
            Console.ReadLine();
        }
    }
}