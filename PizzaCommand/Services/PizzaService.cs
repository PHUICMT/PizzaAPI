using PizzaCommand.Models;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text;

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

            sender();
        }

        public static List<Pizza> GetAll() => Pizzas;

        public static void sender()
        {

            var url = "http://localhost:80/Pizza";
            var request = WebRequest.Create(url);
            request.Method = "POST";

            var json = JsonSerializer.Serialize(Pizzas);
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            Console.WriteLine("Byte Arr : "+byteArray);

            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Console.WriteLine("Byte ContentLength : "+request.ContentLength);

            using var reqStream = request.GetRequestStream();
            reqStream.Write(byteArray, 0, byteArray.Length);
            Console.WriteLine("reqStream : "+reqStream);
            reqStream.Close();

            using var response = request.GetResponse();
            using var respStream = response.GetResponseStream();
            Console.WriteLine("response : "+response);

            using var reader = new StreamReader(respStream);
            Console.WriteLine("reader : "+reader);
            string data = reader.ReadToEnd();
            Console.WriteLine("data : "+data.ToString());
        }
    }
}