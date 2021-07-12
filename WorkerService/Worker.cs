using WorkerService.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        public static DotPizza convertedMessage { get; set; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() => Received());
            }
        }
        async public static void Received()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "pizzaAPI",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Get message : " + message);
                    await Task.Run(() => Tranfrom(message));
                    await Task.Run(() => Insert(convertedMessage));
                };
                channel.BasicConsume(queue: "pizzaAPI",
                                    autoAck: true,
                                    consumer: consumer);
                channel.QueuePurge("pizzaAPI");
            }
        }
        async public static void Tranfrom(string inputMessage)
        {
            Pizza message = JsonSerializer.Deserialize<Pizza>(inputMessage);
            convertedMessage = new DotPizza
            {
                Guid = message.Guid,
                Information = "Name:" + message.Name + " | IsGlutenFree:" + message.IsGlutenFree
            };
        }

        async public static void Insert(DotPizza newPizza)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
               new ConfigurationOptions
               {
                   EndPoints = { "redis:6379" }
               });
            var db = redis.GetDatabase();
            string key = newPizza.Guid;
            await Task.Run(() => db.StringSet(key, JsonSerializer.Serialize(newPizza)));
        }
    }
}
