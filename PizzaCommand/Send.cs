using System;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using PizzaCommand.Models;

namespace Send
{
    class Send
    {
        static void Main(string[] args)
        {
            String jsonMessage = JsonConvert.SerializeObject(new Pizza());//Test
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "sender",
                                                     durable: false,
                                                     exclusive: false,
                                                     autoDelete: false,
                                                     arguments: null);

                    var body = Encoding.UTF8.GetBytes(jsonMessage);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "sender",
                                         basicProperties: null,
                                         body: body);
                }
            }
        }
    }
}
