using PizzaCommand.Models;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using System;

namespace PizzaCommand.Services
{
    public class PizzaService
    {
        public void SendMessage(Pizza pizza)
        {
            pizza.Guid = Guid.NewGuid().ToString();
            var newPizza = JsonSerializer.Serialize(pizza);
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "pizzaAPI",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var body = Encoding.UTF8.GetBytes(newPizza);

                channel.BasicPublish(exchange: "",
                                    routingKey: "pizzaAPI",
                                    basicProperties: null,
                                    body: body);
            }
        }
    }
}