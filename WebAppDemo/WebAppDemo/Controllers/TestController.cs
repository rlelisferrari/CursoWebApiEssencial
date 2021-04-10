using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace WebAppDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] {"teste", "teste2"};
        }

        [HttpGet("pessoas")]
        public ActionResult<IEnumerable<Pessoa>> GetPessoas()
        {
            return new[]
            {
                new Pessoa {Nome = "Rafael"},
                new Pessoa {Nome = "Rafaela"}
            };
        }
    }

    public class Pessoa
    {
        public string Nome;
    }
}