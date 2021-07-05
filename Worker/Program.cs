using Worker.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Worker
{
    class Program
    {
        private Pizza message { get; set; }
        public DotPizza convertedMessage { get; set; }
        public static void Received () {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "pizzaAPI",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    message = JsonSerializer.Deserialize(Encoding.UTF8.GetString(body));
                    
                };
                channel.BasicConsume(queue: "pizzaAPI",
                                    autoAck: true,
                                    consumer: consumer);
            }
        }

        public static void Tranfrom () {
            convertedMessage = new DotPizza{
                Id = message.Id,
                Information = "Name:" + message.Name + " | IsGlutenFree:" + message.IsGlutenFree
            };
        }
    }
}
