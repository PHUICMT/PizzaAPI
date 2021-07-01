using PizzaQuery.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text.Json;

namespace PizzaQuery.Services
{
    public static class PizzaService
    {
        static List<Pizza> Pizzas { get; set; }
        static int nextId = 3;
        static PizzaService()
        {
            Pizzas = new List<Pizza>
            {
                new Pizza { Id = 1, Name = "Classic Italian", IsGlutenFree = true},
                new Pizza { Id = 2, Name = "Veggie",  IsGlutenFree = false}
            };
        }

        public static List<Pizza> GetAll() => Pizzas;

        public static Pizza Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);

        public static void Add(Pizza pizza)
        {
            Pizzas.Add(pizza);
        }
    }
}