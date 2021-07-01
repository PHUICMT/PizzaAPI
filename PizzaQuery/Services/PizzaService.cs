using PizzaQuery.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Linq;

namespace PizzaQuery.Services
{
    public static class PizzaService
    {
        static List<DotPizza> Pizzas { get; }
        static int nextId = 3;
        static PizzaService()
        {
            Pizzas = new List<DotPizza>
            {
                new DotPizza { Id = 1, Information = "Classic Italian"},
                new DotPizza { Id = 2, Information = "Veggie"}
            };
        }

        public static List<DotPizza> GetAll() => Pizzas;

        public static DotPizza Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);

        async public static void receive () {
            using var client = new HttpClient();
            var content = await client.GetStringAsync("http://localhost:81/Pizza");

            Console.WriteLine("Content : "+content);
        }
    }
}