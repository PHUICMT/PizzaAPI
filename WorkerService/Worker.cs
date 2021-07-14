using WorkerService.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        public static Serilog.ILogger Log { get; set; }
        public Worker()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.Development.json")
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
               .Build();

            Log = new LoggerConfiguration()
                  .ReadFrom.Configuration(configuration)
                  .CreateLogger();
        }




        // test
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
            var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
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
                    // Console.WriteLine("Get message : " + message);
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
            Log.Information("|Guid: [" + convertedMessage.Guid + "] STEP 3 Recieved. Time: " + DateTime.Now + " " + DateTime.Now.Millisecond + "ms");

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
            Log.Information("|Guid: [" + key + "] STEP 4 Send to Redis. Time: " + DateTime.Now + " " + DateTime.Now.Millisecond + "ms");

        }
    }
}
