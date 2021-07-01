using PizzaWorker.Models;
using System.Collections.Generic;
using System.Linq;

namespace PizzaWorker.Services
{
    public static class PizzaService
    {
        static List<DotPizza> DotPizzas { get; }
        static PizzaService()
        {
        }

        public static void TransformData(List<Pizza> pizzas)
        {
            foreach (var data in pizzas) {
                var converted = new DotPizza();
                converted.Id = data.Id;
                converted.Information = data.Name + "| IsGlutenFree : " + data.IsGlutenFree;
                DotPizzas.Add(converted);
            }
        }
        public static List<DotPizza> GetAll() => DotPizzas;

    }
}