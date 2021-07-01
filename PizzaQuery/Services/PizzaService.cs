using PizzaQuery.Models;
using System.Collections.Generic;
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
                new DotPizza { Id = 1, Information = "Classic Italian" },
                new DotPizza { Id = 2, Information = "Veggie" }
            };
        }

        public static List<DotPizza> GetAll() => Pizzas;

        public static DotPizza Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);
    }
}