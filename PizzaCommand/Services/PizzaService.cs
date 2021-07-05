using PizzaCommand.Models;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System;
using RabbitMQ.Client;
using System.Text;

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
            // var data = new StringContent(newPizza, Encoding.UTF8, "application/json");

            // var url = "http://localhost:80/query/Pizza";
            // using var client = new HttpClient();

            // var response = await client.PostAsync(url, data);
            // string result = response.Content.ReadAsStringAsync().Result;
            // Console.WriteLine(response);
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "pizzaAPI",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                // string message = "Kwaii";
                var body = Encoding.UTF8.GetBytes(newPizza);

                channel.BasicPublish(exchange: "",
                                    routingKey: "pizzaAPI",
                                    basicProperties: null,
                                    body: body);
            }
            }
        }
        // public List<Pizza> GetAll() => Pizzas; 
    
}