using PizzaQuery.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.Json;
using System;

namespace PizzaQuery.Services
{
    public static class PizzaService
    {
        static List<Pizza> Pizzas { get; }
        static int nextId = 3;
        static PizzaService()
        {
            Pizzas = new List<Pizza>
            {
                new Pizza { Id = 1, Name = "Classic Italian", IsGlutenFree = false },
                new Pizza { Id = 2, Name = "Veggie", IsGlutenFree = true }
            };
        }

        public static List<Pizza> GetAll() => Pizzas;

        public static Pizza Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);

        public static void receive()
        {
            var url = "http://localhost:81/Pizza";
            var request = WebRequest.Create(url);
            request.Method = "GET";
            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();

            Console.WriteLine("data : " + data.ToString());
            //Pizzas.Add(JsonSerializer.Deserialize<Pizza>(data));

        }
    }
}