using Common;
using Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Sender2
{
    class Program
    {
        static void Main(string[] args)
        {
            using var connection = new RabbitMQConnection();
            var channel = connection.GetChannel();

            // Declarar exchange
            channel.ExchangeDeclare(exchange: "user_exchange", type: ExchangeType.Direct);

            // Enviar mensagem
            var user = new UserMessage
            {
                FullName = "João Silva",
                Address = "Rua das Flores, 123, São Paulo, SP",
                RG = "12.345.678-9",
                CPF = "123.456.789-00",
                RegistrationTimestamp = DateTime.Now
            };

            var message = JsonConvert.SerializeObject(user);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "user_exchange",
                routingKey: "user.validation",
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[Sender2] Mensagem enviada: {message}");
            Console.ReadLine();
        }
    }
}