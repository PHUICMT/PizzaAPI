using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PizzaWorker.Models;
using PizzaWorker.Services;

namespace PizzaWorker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaController : ControllerBase
    {
        public PizzaController()
        {
        }

        [HttpGet]
        public ActionResult<List<DotPizza>> GetAll() => PizzaService.GetAll();

        [HttpPost]
        public IActionResult Transform(List<Pizza> pizzas)
        {
            PizzaService.TransformData(pizzas);
            return CreatedAtAction(nameof(Transform), new { total =  pizzas.Count}, pizzas);
        }

    }
}