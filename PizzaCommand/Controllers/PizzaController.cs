using Microsoft.AspNetCore.Mvc;
using PizzaCommand.Models;
using PizzaCommand.Services;

namespace PizzaCommand.Controllers
{

    [ApiController]
    [Route("command/[controller]")]
    public class PizzaController : ControllerBase
    {
        private PizzaService _pizzaService;
        public PizzaController()
        {
            _pizzaService = new PizzaService();
        }

        // POST action
        [HttpPost]
        public IActionResult Create(Pizza pizza)
        {
            _pizzaService.SendMessage(pizza);
            return CreatedAtAction(nameof(Create), new { id = pizza.Guid }, pizza);
        }
    }
}