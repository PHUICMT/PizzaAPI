using PizzaCommand.Models;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using System;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Configuration;
using System.IO;
using Serilog.Core;

namespace PizzaCommand.Services
{
    public class PizzaService
    {
        public PizzaService()
        {
        }
        public Serilog.ILogger CreateLog()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.Development.json")
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
               .Build();

            var logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(configuration)
                  .CreateLogger();
            return logger;
        }
        public void SendMessage(Pizza pizza)
        {
            var Log = CreateLog();
            pizza.Guid = Guid.NewGuid().ToString();
            Log.Information("|Guid: [" + pizza.Guid + "] STEP 1 Post. Time: " + DateTime.Now + " " + DateTime.Now.Millisecond + "ms");
            var newPizza = JsonSerializer.Serialize(pizza);
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
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
                Log.Information("|Guid: [" + pizza.Guid + "] STEP 2 Service to rabbitmq. Time: " + DateTime.Now + " " + DateTime.Now.Millisecond + "ms");
            }
        }
    }
}