using Common;
using Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Validation
{
    class Program
    {
        static void Main(string[] args)
        {
            using var connection = new RabbitMQConnection();
            var channel = connection.GetChannel();

            // Declarar exchanges
            channel.ExchangeDeclare(exchange: "fruit_exchange", type: ExchangeType.Direct);
            channel.ExchangeDeclare(exchange: "user_exchange", type: ExchangeType.Direct);

            // Declarar filas
            channel.QueueDeclare(queue: "validation_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: "validation_queue", exchange: "fruit_exchange", routingKey: "fruit.validation");
            channel.QueueBind(queue: "validation_queue", exchange: "user_exchange", routingKey: "user.validation");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                if (routingKey == "fruit.validation")
                {
                    var fruit = JsonConvert.DeserializeObject<FruitMessage>(message);
                    // Validação simples
                    if (!string.IsNullOrEmpty(fruit.FruitName) && !string.IsNullOrEmpty(fruit.Description))
                    {
                        channel.BasicPublish(
                            exchange: "fruit_exchange",
                            routingKey: "fruit.receiver",
                            basicProperties: null,
                            body: body
                        );
                        Console.WriteLine($"[Validation] Fruta validada: {message}");
                    }
                }
                else if (routingKey == "user.validation")
                {
                    var user = JsonConvert.DeserializeObject<UserMessage>(message);
                    // Validação simples
                    if (!string.IsNullOrEmpty(user.FullName) && !string.IsNullOrEmpty(user.CPF))
                    {
                        channel.BasicPublish(
                            exchange: "user_exchange",
                            routingKey: "user.receiver",
                            basicProperties: null,
                            body: body
                        );
                        Console.WriteLine($"[Validation] Usuário validado: {message}");
                    }
                }
            };

            channel.BasicConsume(queue: "validation_queue", autoAck: true, consumer: consumer);
            Console.WriteLine("[Validation] Aguardando mensagens...");
            Console.ReadLine();
        }
    }
}