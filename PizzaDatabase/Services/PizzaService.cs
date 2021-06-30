using PizzaCommand.Models;
using System.Collections.Generic;
using System.Linq;

namespace PizzaCommand.Services
{
    public static class PizzaService
    {
        static List<Pizza> Pizzas { get; }
        static List<DotPizza> DotPizzas { get; }
        static int nextId = 3;
        static PizzaService(Pizza pizza)
        {
            Pizzas = pizza;
        }
        public static void TransformData()
        {
            foreach ((int id, string Name, bool IsGlutenFree) in Pizzas)
            {
                DotPizzas.Add(new DotPizza { Id = id, Information = (Name + IsGlutenFree)});
            }
        }
        public static List<DotPizza> GetAll() => DotPizzas;
    }
}