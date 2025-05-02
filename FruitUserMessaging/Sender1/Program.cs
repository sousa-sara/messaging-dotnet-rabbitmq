using Common;
using Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Sender1
{
    class Program
    {
        static void Main(string[] args)
        {
            using var connection = new RabbitMQConnection();
            var channel = connection.GetChannel();

            // Declarar exchange
            channel.ExchangeDeclare(exchange: "fruit_exchange", type: ExchangeType.Direct);

            // Enviar mensagem
            var fruit = new FruitMessage
            {
                FruitName = "Manga",
                Description = "Fruta tropical, doce e suculenta, comum no verão.",
                Timestamp = DateTime.Now
            };

            var message = JsonConvert.SerializeObject(fruit);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "fruit_exchange",
                routingKey: "fruit.validation",
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[Sender1] Mensagem enviada: {message}");
            Console.ReadLine();
        }
    }
}