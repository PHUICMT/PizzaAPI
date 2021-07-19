using WorkerService.Models;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using Serilog;
using System.Net;
using System.IO;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private static Random _random = new Random();
        private static int ranNum;

        public static DotPizza convertedMessage { get; set; }

        public Worker()
        {
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() => Received());
                await Task.Run(() => ReceivedRandomNumber());
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
                    ranNum = _random.Next(1, 4);
                    // Log.Information("Random:" + ranNum);
                    await Task.Delay(ranNum * 1000);

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    await Task.Run(() => Tranfrom(message));
                    await Task.Run(() => Insert(convertedMessage));


                };
                channel.BasicConsume(queue: "pizzaAPI",
                                    autoAck: true,
                                    consumer: consumer);
                channel.QueuePurge("pizzaAPI");
            }
        }

        async public static void ReceivedRandomNumber()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "pizzaAPINumber",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    ranNum += int.Parse(message);
                };
                channel.BasicConsume(queue: "pizzaAPINumber",
                                    autoAck: true,
                                    consumer: consumer);
                channel.QueuePurge("pizzaAPINumber");
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
        public static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
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
            Log.Information("================= TOTAL Random Number IS [" + ranNum + "] =================");
        }
    }
}
