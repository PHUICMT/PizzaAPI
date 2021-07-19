using PizzaCommand.Models;
using RabbitMQ.Client;
using Serilog;
using System.Text;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace PizzaCommand.Services
{
    public class PizzaService
    {
        private readonly Random _random = new Random();
        private static int ranNum;
        public PizzaService()
        {
        }
        async public Task SendMessage(Pizza pizza)
        {
            ranNum = _random.Next(1, 4);
            // Log.Information("Random:" + ranNum);
            await Task.Delay(ranNum * 1000);

            Log.Information("|Guid: [" + pizza.Guid + "] STEP 1 Post. Time: " + DateTime.Now + " " + DateTime.Now.Millisecond + "ms");
            var newPizza = JsonSerializer.Serialize(pizza);
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            {
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
                    Log.Information("|Guid: [" + pizza.Guid + "] STEP 2 Service to rabbitmq. Time: " + DateTime.Now + " " + DateTime.Now.Millisecond + "ms");
                }

            }
        }

        async public Task SendRandomNumber(int randomNumber)
        {
            ranNum += randomNumber;

            var newranNum = JsonSerializer.Serialize(ranNum);
            var bodyRandomNumber = Encoding.UTF8.GetBytes(newranNum);

            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())//Send Random Number
                {
                    channel.QueueDeclare(queue: "pizzaAPINumber",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    channel.BasicPublish(exchange: "",
                                        routingKey: "pizzaAPINumber",
                                        basicProperties: null,
                                        body: bodyRandomNumber);
                }
            }
        }
    }
}