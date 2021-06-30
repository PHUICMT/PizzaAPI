using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PizzaCommand.Models;
using PizzaCommand.Services;

namespace PizzaCommand.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaController : ControllerBase
    {
        public PizzaController()
        {
        }

        [HttpPost]
        public IActionResult Transform(List<Pizza> pizzas)
        {
            PizzaService(pizzas).TransformData();
            return CreatedAtAction(nameof(Transform), new { id = pizzas.Id }, pizzas);
        }

        [HttpGet]
        public ActionResult<List<Pizza>> GetAll() => PizzaService.GetAll();
    }
}