using PizzaQuery.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
// using Newtonsoft.Json;

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

        // public static Pizza Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);

        async public static void receive () {
            using var client = new HttpClient();
            var content = await client.GetStringAsync("http://localhost:81/Pizza");

            Console.WriteLine("Content : "+content);
        }
    }
}