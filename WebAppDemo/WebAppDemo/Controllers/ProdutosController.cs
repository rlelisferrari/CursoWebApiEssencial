using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebAppDemo.Context;
using WebAppDemo.Models;

namespace WebAppDemo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ILogger logger;

        public ProdutosController(AppDbContext context, ILogger<ProdutosController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [HttpGet("/first")]
        public async Task<ActionResult<Produto>> GetFirst()
        {
            var produto = await this.context.Produtos.ToListAsync();
            this.logger.LogInformation($"Método GetFirst");
            this.logger.LogInformation(produto.FirstOrDefault().ToString());
            return produto.FirstOrDefault();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            var produtos = await this.context.Produtos.AsNoTracking().ToListAsync();
            this.logger.LogInformation($"Método GET: Numero de produtos {produtos.Count}");
            //Desabilitar o rastreamento das consultas (aumenta o desempenho da api)
            return produtos;
        }

        //{id}/{param?}: parâmetro opcional
        //{id}/{param=Mac}: se não passar parâmetro recebe parâmetro default "Mac"
        //{id:int:min(1)}: restrição de rotas valores inteiros >= 1
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public IActionResult Get([FromQuery] int id)
        {
            var produto = this.context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
                return NotFound();
            return Ok(produto);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            produto.DataCadastro = DateTime.Now;
            this.context.Add(produto);
            this.context.SaveChanges();
            return new CreatedAtRouteResult("ObterProduto", new {id = produto.ProdutoId}, produto);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            //altera o estado da entidade oara modified
            this.context.Entry(produto).State = EntityState.Modified;
            //aplica as alterações no banco
            this.context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            //busca direto no banco de dados
            var produto = this.context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            //1º busca em memória, se não achar busca no banco
            //var produto = this.context.Produtos.Find(id);

            if (produto == null)
                return NotFound();

            this.context.Produtos.Remove(produto);
            this.context.SaveChanges();
            return produto;
        }
    }
}