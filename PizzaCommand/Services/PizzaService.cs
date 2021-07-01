using PizzaCommand.Models;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;

namespace PizzaCommand.Services
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
        public static void Add(Pizza pizza)
        {
            pizza.Id = nextId++;
            Pizzas.Add(pizza);
        }

        public static string convert()
        {
            string DotPizzas = "";
            foreach (var data in Pizzas)
            {
                var newPizza = new DotPizza();
                newPizza.Id = data.Id;
                newPizza.Information = data.Name + " IsGlutenFree : " + data.IsGlutenFree;
                DotPizzas += JsonSerializer.Serialize(newPizza);
            }

            return DotPizzas;
        }

        async public static void sender()
        {
            var data = new StringContent(convert(), Encoding.UTF8, "application/json");

            var url = "http://localhost:80/Pizza";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
        }
        public static List<Pizza> GetAll() => Pizzas;
    }
}