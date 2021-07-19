using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaCommand.Models;
using PizzaCommand.Services;
using Serilog;

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
            ranNum = _random.Next(1,4);
            // Log.Information("Random:" + ranNum);
            await Task.Delay(ranNum*1000);
            
            await _pizzaService.SendMessage(pizza);

            await _pizzaService.SendRandomNumber(ranNum);

            return CreatedAtAction(nameof(Create), new { id = pizza.Guid }, pizza);
        }
    }
}