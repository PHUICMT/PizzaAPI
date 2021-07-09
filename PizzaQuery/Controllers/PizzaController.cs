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
        

        [HttpGet("{guid}")]
        public ActionResult<DotPizza> Get(string guid)
        {
            var pizza = PizzaService.Get(guid);

            if (pizza == null)
                return NotFound();

            return pizza;
        }
    }
}