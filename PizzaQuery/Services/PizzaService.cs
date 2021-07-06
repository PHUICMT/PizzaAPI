using PizzaQuery.Models;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace PizzaQuery.Services
{
    public static class PizzaService
    {
        static List<DotPizza> Pizzas { get; set; }

        public static List<DotPizza> GetAll(){
            Connect();
            return Pizzas;
        } 

        public static DotPizza Get(int id) {
            Connect();
            return Pizzas.FirstOrDefault(p => p.Id == id);
        }

        public static bool Connect()
        {
            Pizzas =  new List<DotPizza>();
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
               new ConfigurationOptions
               {
                   EndPoints = { "localhost:6379" }
               });
            var db = redis.GetDatabase();
            var endPoint = redis.GetEndPoints().First();
            RedisKey[] keys = redis.GetServer(endPoint).Keys(pattern: "*").ToArray();
            var server = redis.GetServer(endPoint);
            foreach (var key in server.Keys())
            {
                string value = db.StringGet(key);
                DotPizza newDotPizza = JsonSerializer.Deserialize<DotPizza>(value);
                Pizzas.Add(newDotPizza);
                // Console.WriteLine(value);
            }
            return true;
        }

    }
}