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
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() => Received());
                // await Task.Delay(1000, stoppingToken);
            }
        }
        async public static void Received()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
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
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Message :  " + message);
                    Tranfrom(message);
                    Insert(convertedMessage);
                };
                channel.BasicConsume(queue: "pizzaAPI",
                                    autoAck: true,
                                    consumer: consumer);

            }
        }
        async public static void Tranfrom(string inputMessage)
        {
            Pizza message = JsonSerializer.Deserialize<Pizza>(inputMessage);
            convertedMessage = new DotPizza
            {
                Id = message.Id,
                Information = "Name:" + message.Name + " | IsGlutenFree:" + message.IsGlutenFree
            };
            Console.WriteLine("+++++ " + JsonSerializer.Serialize(convertedMessage));
        }

        async public static void Insert(DotPizza newPizza)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
               new ConfigurationOptions
               {
                   EndPoints = { "localhost:6379" }
               });
            var db = redis.GetDatabase();
            string key = newPizza.Id.ToString();
            await Task.Run(() => db.StringSet(key, JsonSerializer.Serialize(newPizza)));
        }

        // async public static void RedisConnection()
        // {
        //     ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
        //        new ConfigurationOptions
        //        {
        //            EndPoints = { "localhost:6379" }
        //        });

        //     var db = redis.GetDatabase();

        //     var keys = redis.GetServer("localhost", 6379).Keys();

        //     string[] keysArr = keys.Select(key => (string)key).ToArray();

        //     foreach (string key in keysArr)
        //     {
        //         Console.WriteLine(db.StringGet(key));
        //     }

        //     var pong = await db.PingAsync();
        //     Console.WriteLine(redis);

        // }
    }
}
