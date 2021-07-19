using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaCommand.Models;
using PizzaCommand.Services;
using StackExchange.Redis;

namespace PizzaCommand.Controllers
{

    [ApiController]
    [Route("command/[controller]")]
    public class PizzaController : ControllerBase
    {
        private readonly Random _random = new Random();
        private static int ranNum;
        private PizzaService _pizzaService;
        public PizzaController()
        {
            _pizzaService = new PizzaService();
        }

        // POST action
        [HttpPost]
        async public Task<ActionResult> Create(Pizza pizza)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
               new ConfigurationOptions
               {
                   EndPoints = { "redis:6379" }
               });
            var db = redis.GetDatabase();

            pizza.Guid = Guid.NewGuid().ToString();
            string key = "[Time]"+pizza.Guid;

            await Task.Run(() => db.StringSet(key, JsonSerializer.Serialize(DateTime.Now)));

            ranNum = _random.Next(0, 4);
            await Task.Delay(ranNum * 1000);

            await _pizzaService.SendMessage(pizza);
            await _pizzaService.SendRandomNumber(ranNum);

            return CreatedAtAction(nameof(Create), new { id = pizza.Guid }, pizza);
        }
    }
}