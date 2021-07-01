using PizzaCommand.Models;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System;

namespace PizzaCommand.Services
{
    public class PizzaService
    {
        List<Pizza> Pizzas { get; }
        int nextId = 3;
        public PizzaService()
        {
            Pizzas = new List<Pizza>
            {
                new Pizza { Id = 1, Name = "Classic Italian", IsGlutenFree = false },
                new Pizza { Id = 2, Name = "Veggie", IsGlutenFree = true }
            };
        }
        public void Add(Pizza pizza)
        {
            // pizza.Id = nextId++;
            Pizzas.Add(pizza);
        }

        async public void sender()
        {
            var newPizza = JsonSerializer.Serialize(Pizzas[Pizzas.Count - 1]);
            var data = new StringContent(newPizza, Encoding.UTF8, "application/json");

            var url = "http://localhost:80/query/Pizza";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);
            string result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(response);
        }
        public List<Pizza> GetAll() => Pizzas;
    }
}