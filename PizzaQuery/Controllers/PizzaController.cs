using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PizzaQuery.Models;
using PizzaQuery.Services;
using System;

namespace PizzaQuery.Controllers
{
    [ApiController]
    [Route("query/[controller]")]
    public class PizzaController : ControllerBase
    {
        public PizzaController()
        {
        }

        [HttpGet]
        public ActionResult<List<DotPizza>> GetAll() => PizzaService.GetAll();
        

        [HttpGet("{id}")]
        public ActionResult<DotPizza> Get(int id)
        {
            var pizza = PizzaService.Get(id);

            if (pizza == null)
                return NotFound();

            return pizza;
        }
    }
}